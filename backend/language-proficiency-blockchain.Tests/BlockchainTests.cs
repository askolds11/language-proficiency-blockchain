using System.Security.Cryptography;
using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.Database.Models;
using language_proficiency_blockchain.HashModels;
using language_proficiency_blockchain.HashModels.v1;
using language_proficiency_blockchain.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace language_proficiency_blockchain.Tests;

[NotInParallel]
[ClassDataSource<RsaKeyFixture>(Shared = SharedType.PerClass)]
internal class BlockchainTests(RsaKeyFixture fixture) : BaseIntegrationTest
{
    [Test]
    public async Task ProposeBlockAsync_valid_signature_returns_signed_hash()
    {
        using var scope = Factory.Services.CreateScope();
        var crypto = scope.ServiceProvider.GetRequiredService<CryptoService>();

        var block = new HashableInstitutionV1([1, 2, 3], Guid.NewGuid(), "inst");
        var canonical = block with { PrevHash = [] };
        var hash = Hasher.HashBlock(canonical);
        var signed = fixture.PrivateKey.SignData(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var publicKey = fixture.PublicKey.ExportRSAPublicKey();

        var result = new BlockchainService(scope.ServiceProvider.GetRequiredService<AppDbContext>(), crypto)
            .ProposeBlockAsync(block, hash, signed, publicKey);

        await Assert.That(result.Length).IsGreaterThan(0);
    }

    [Test]
    public async Task ProposeBlockAsync_invalid_signature_throws()
    {
        using var scope = Factory.Services.CreateScope();
        var crypto = scope.ServiceProvider.GetRequiredService<CryptoService>();

        var block = new HashableInstitutionV1([], Guid.NewGuid(), "inst");
        var canonical = block with { PrevHash = [] };
        var hash = Hasher.HashBlock(canonical);
        var wrongSignature =
            fixture.PrivateKey.SignData([1, 2, 3], HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var publicKey = fixture.PublicKey.ExportRSAPublicKey();

        var svc = new BlockchainService(scope.ServiceProvider.GetRequiredService<AppDbContext>(), crypto);

        await Assert.That(() => svc.ProposeBlockAsync(block, hash, wrongSignature, publicKey))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AddInstitutionBlockAsync_sets_block_and_links_institution()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var svc = scope.ServiceProvider.GetRequiredService<BlockchainService>();

        var instId = await CreateInstitutionAsync(db, "inst-A");
        var signerId = await CreateInstitutionAsync(db, "inst-B");

        var (prevId, prevHash) = await GetTailAsync(db);
        var hashable = new HashableInstitutionV1(prevHash, instId, "Institution A");
        var hash = Hasher.HashBlock(hashable);
        var signatures = new[]
        {
            new BlockchainService.BlockSignature(signerId,
                fixture.PrivateKey.SignData(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
        };

        var block = await svc.AddInstitutionBlockAsync(Guid.NewGuid(), instId, "Institution A", signatures);

        var reloaded = await db.Institutions.FindAsync(instId);
        await Assert.That(reloaded).IsNotNull();
        await Assert.That(reloaded!.BlockId).IsEqualTo(block.Id);
        await Assert.That(block.PrevId).IsEqualTo(prevId);
        await Assert.That(block.Hash).IsEquivalentTo(hash);
    }

    [Test]
    public async Task AddTestBlockAsync_creates_test_and_block()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var svc = scope.ServiceProvider.GetRequiredService<BlockchainService>();

        var instId = await CreateInstitutionAsync(db, "inst-A");
        var signerId = await CreateInstitutionAsync(db, "inst-B");

        var (prevId, prevHash) = await GetTailAsync(db);
        var testId = Guid.NewGuid();
        var hashable = new HashableTestV1(prevHash, instId, testId, "100");
        var hash = Hasher.HashBlock(hashable);
        var signatures = new[]
        {
            new BlockchainService.BlockSignature(signerId,
                fixture.PrivateKey.SignData(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
        };

        var block = await svc.AddTestBlockAsync(Guid.NewGuid(), testId, instId, "100", "Lang Test", signatures);

        var test = await db.Tests.FindAsync(testId);
        await Assert.That(test).IsNotNull();
        await Assert.That(test!.BlockId).IsEqualTo(block.Id);
        await Assert.That(block.PrevId).IsEqualTo(prevId);
    }

    [Test]
    public async Task AddTestResultBlockAsync_creates_result_and_block()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var svc = scope.ServiceProvider.GetRequiredService<BlockchainService>();

        var instId = await CreateInstitutionAsync(db, "inst-A");
        var signerId = await CreateInstitutionAsync(db, "inst-B");

        var testId = Guid.NewGuid();
        {
            var (prevId, prevHash) = await GetTailAsync(db);
            var hashableTest = new HashableTestV1(prevHash, instId, testId, "100");
            var hashTest = Hasher.HashBlock(hashableTest);
            var sigsTest = new[]
            {
                new BlockchainService.BlockSignature(signerId,
                    fixture.PrivateKey.SignData(hashTest, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
            };
            await svc.AddTestBlockAsync(Guid.NewGuid(), testId, instId, "100", "Lang Test", sigsTest);
        }

        var (prevIdResult, prevHashResult) = await GetTailAsync(db);
        var resultId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var hashableResult = new HashableTestResultV1(prevHashResult, testId, resultId, "85");
        var hashResult = Hasher.HashBlock(hashableResult);
        var sigsResult = new[]
        {
            new BlockchainService.BlockSignature(signerId,
                fixture.PrivateKey.SignData(hashResult, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
        };

        var block = await svc.AddTestResultBlockAsync(Guid.NewGuid(), resultId, testId, studentId, "85",
            DateTimeOffset.UtcNow, sigsResult);

        var result = await db.TestResults.FindAsync(resultId);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.BlockId).IsEqualTo(block.Id);
        await Assert.That(block.PrevId).IsEqualTo(prevIdResult);
    }

    [Test]
    public async Task AddBlock_fails_when_quorum_not_met()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var svc = scope.ServiceProvider.GetRequiredService<BlockchainService>();

        var instA = await CreateInstitutionAsync(db, "inst-A");
        var instB = await CreateInstitutionAsync(db, "inst-B");
        await CreateInstitutionAsync(db, "inst-C");

        var (prevId, prevHash) = await GetTailAsync(db);
        var hashable = new HashableTestV1(prevHash, instA, Guid.NewGuid(), "100");
        var hash = Hasher.HashBlock(hashable);

        // quorum for 3 institutions is 2; provide only one valid signature
        var signatures = new[]
        {
            new BlockchainService.BlockSignature(instB,
                fixture.PrivateKey.SignData(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
        };

        await Assert.That(() =>
                svc.AddTestBlockAsync(Guid.NewGuid(), Guid.NewGuid(), instA, "100", "Lang Test", signatures))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AddTestResultBlockAsync_creates_student_when_missing()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var svc = scope.ServiceProvider.GetRequiredService<BlockchainService>();

        var instId = await CreateInstitutionAsync(db, "inst-A");
        var signerId = await CreateInstitutionAsync(db, "inst-B");

        var testId = Guid.NewGuid();
        {
            var (prevId, prevHash) = await GetTailAsync(db);
            var hashableTest = new HashableTestV1(prevHash, instId, testId, "100");
            var hashTest = Hasher.HashBlock(hashableTest);
            var sigsTest = new[]
            {
                new BlockchainService.BlockSignature(signerId,
                    fixture.PrivateKey.SignData(hashTest, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
            };
            await svc.AddTestBlockAsync(Guid.NewGuid(), testId, instId, "100", "Lang Test", sigsTest);
        }

        var missingStudentId = Guid.NewGuid();
        var (prevIdResult, prevHashResult) = await GetTailAsync(db);
        var resultId = Guid.NewGuid();
        var hashableResult = new HashableTestResultV1(prevHashResult, testId, resultId, "91");
        var hashResult = Hasher.HashBlock(hashableResult);
        var sigsResult = new[]
        {
            new BlockchainService.BlockSignature(signerId,
                fixture.PrivateKey.SignData(hashResult, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
        };

        var block = await svc.AddTestResultBlockAsync(
            Guid.NewGuid(),
            resultId,
            testId,
            missingStudentId,
            "91",
            DateTimeOffset.UtcNow,
            sigsResult);

        var student = await db.Students.FindAsync(missingStudentId);
        await Assert.That(student).IsNotNull();
        await Assert.That(block.PrevId).IsEqualTo(prevIdResult);
    }

    private async Task<Guid> CreateInstitutionAsync(AppDbContext db, string address)
    {
        var id = Guid.NewGuid();
        var entity = new InstitutionEntity
        {
            Id = id,
            BlockId = null,
            Address = address,
            PublicKeyPem = fixture.PublicKey.ExportRSAPublicKey()
        };
        db.Institutions.Add(entity);
        await db.SaveChangesAsync();
        return id;
    }

    private static async Task<(Guid prevId, byte[] prevHash)> GetTailAsync(AppDbContext db)
    {
        var tail = await db.Blocks
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id != e.PrevId && !db.Blocks.Any(x => x.PrevId == e.Id));

        if (tail is not null)
        {
            return (tail.Id, tail.Hash);
        }

        var genesis = await db.Blocks
                          .AsNoTracking()
                          .FirstOrDefaultAsync(e => e.Id == e.PrevId)
                      ?? throw new InvalidOperationException("Blockchain is not initialized.");

        return (genesis.Id, genesis.Hash);
    }
}