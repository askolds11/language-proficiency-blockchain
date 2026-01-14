using System.Security.Cryptography;
using System.Text.Json;
using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.Database.Models;
using language_proficiency_blockchain.HashModels.Interfaces;
using language_proficiency_blockchain.requests.Blockchain;
using language_proficiency_blockchain.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace language_proficiency_blockchain.Tests;

[ClassDataSource<RsaKeyFixture>(Shared = SharedType.PerClass)]
[NotInParallel]
internal class InternalServiceProposalTests(RsaKeyFixture rsaKeyFixture) : BaseIntegrationTest
{
    [Test]
    public async Task ProposeInstitutionBlockAsync_ThrowsWhenInstitutionNotFound()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var mockNodeRepository = Substitute.For<INodeRepository>();

        var service = new InternalService(
            db,
            mockNodeRepository,
            null!,
            null!,
            rsaKeyFixture.RsaOptionsMonitor);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ProposeInstitutionBlockAsync(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Test Institution",
                CancellationToken.None));
    }

    [Test]
    public async Task ProposeInstitutionBlockAsync_ThrowsWhenInstitutionAlreadyHasBlock()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var institutionId = Guid.CreateVersion7();
        var blockId = Guid.CreateVersion7();

        var firstBlock = await db.Blocks.FirstAsync();

        db.Blocks.Add(new BlockEntity
        {
            Id = blockId,
            Type = BlockType.Institution,
            PrevId = firstBlock.Id,
            PrevHash = firstBlock.Hash,
            Hash = [],
            InstitutionId = null,
            SignedHash = [],
            Timestamp = DateTimeOffset.UtcNow
        });

        db.Institutions.Add(new InstitutionEntity
        {
            Id = institutionId,
            BlockId = blockId,
            Address = "http://test.com",
            PublicKeyPem = []
        });
        await db.SaveChangesAsync();

        var mockNodeRepository = Substitute.For<INodeRepository>();

        var service = new InternalService(
            db,
            mockNodeRepository,
            null!,
            null!,
            rsaKeyFixture.RsaOptionsMonitor);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ProposeInstitutionBlockAsync(
                Guid.CreateVersion7(),
                institutionId,
                "Test Institution",
                CancellationToken.None));
    }

    [Test]
    public async Task ProposeInstitutionBlockAsync_ThrowsWhenNotEnoughSignatures()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var institutionId = Guid.NewGuid();

        db.Institutions.Add(new InstitutionEntity
        {
            Id = institutionId,
            BlockId = null,
            Address = "http://test.com",
            PublicKeyPem = []
        });
        await db.SaveChangesAsync();

        var mockNodeRepository = Substitute.For<INodeRepository>();
        mockNodeRepository.CollectSignaturesAsync(
            Arg.Any<BlockBase>(),
            Arg.Any<byte[]>(),
            Arg.Any<byte[]>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>()
        ).Returns(new List<CollectedSignature>());

        var mockCryptoService = Substitute.For<ICryptoService>();
        mockCryptoService.SignHash(Arg.Any<byte[]>()).Returns([1, 2, 3]);

        var service = new InternalService(
            db,
            mockNodeRepository,
            null!,
            mockCryptoService,
            rsaKeyFixture.RsaOptionsMonitor);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ProposeInstitutionBlockAsync(
                Guid.NewGuid(),
                institutionId,
                "Test Institution",
                CancellationToken.None));
    }

    [Test]
    public async Task ProposeInstitutionBlockAsync_CollectsSignaturesAndAddsBlock()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var crypto = scope.ServiceProvider.GetRequiredService<ICryptoService>();
        var blockchain = scope.ServiceProvider.GetRequiredService<BlockchainService>();
        var mockNodeRepo = Substitute.For<INodeRepository>();

        var institutionId = Guid.NewGuid();
        var signerId = Guid.NewGuid();
        var signerBlockId = Guid.NewGuid();
        
        var firstBlock = await db.Blocks.FirstAsync();
        
        db.Blocks.Add(new BlockEntity
        {
            Id = signerBlockId,
            Type = BlockType.Institution,
            PrevId = firstBlock.Id,
            PrevHash = firstBlock.PrevHash,
            Hash = [1, 2, 3],
            InstitutionId = null,
            SignedHash = [],
            Timestamp = DateTimeOffset.UtcNow
        });

        db.Institutions.Add(new InstitutionEntity
        {
            Id = signerId,
            BlockId = signerBlockId,
            Address = "http://signer.test",
            PublicKeyPem = rsaKeyFixture.PublicKey.ExportRSAPublicKey()
        });

        // Create target institution without block
        db.Institutions.Add(new InstitutionEntity
        {
            Id = institutionId,
            BlockId = null,
            Address = "http://test.test",
            PublicKeyPem = []
        });
        await db.SaveChangesAsync();

        // Configure fake to return valid signature
        mockNodeRepo.CollectSignaturesAsync(
                Arg.Any<BlockBase>(),
                Arg.Any<byte[]>(),
                Arg.Any<byte[]>(),
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var hash = callInfo.ArgAt<byte[]>(1);
                var signedHash = crypto.SignHash(hash);
                return Task.FromResult<IReadOnlyCollection<CollectedSignature>>(
                [
                    new CollectedSignature(signerId, signedHash)
                ]);
            });

        var service = new InternalService(
            db,
            mockNodeRepo,
            blockchain,
            crypto,
            rsaKeyFixture.RsaOptionsMonitor);

        // Act
        var blockId = Guid.NewGuid();
        var result = await service.ProposeInstitutionBlockAsync(
            blockId,
            institutionId,
            "Test Institution",
            CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Type).IsEqualTo(BlockType.Institution);

        // Verify signature collection was called
        await mockNodeRepo.Received(1).CollectSignaturesAsync(
            Arg.Any<BlockBase>(),
            Arg.Any<byte[]>(),
            Arg.Any<byte[]>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>());

        // Verify broadcast was called
        await mockNodeRepo.Received(1).BroadcastFinalizedBlockAsync(
            "blockchain/blocks/institution",
            Arg.Any<AddInstitutionBlockRequest>(),
            Arg.Any<CancellationToken>());

        // Verify institution is now linked to block
        var institution = await db.Institutions.FindAsync(institutionId);
        await Assert.That(institution!.BlockId).IsEqualTo(result.Id);
    }

    [Test]
    public async Task ProposeTestBlockAsync_CollectsSignaturesAndAddsBlock()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var crypto = scope.ServiceProvider.GetRequiredService<ICryptoService>();
        var blockchain = scope.ServiceProvider.GetRequiredService<BlockchainService>();
        var mockNodeRepo = Substitute.For<INodeRepository>();

        var institutionId = Guid.NewGuid();

        // Create institution with block
        var instBlockId = Guid.NewGuid();
        db.Blocks.Add(new BlockEntity
        {
            Id = instBlockId,
            Type = BlockType.Institution,
            PrevId = new Guid("019b6749-15c6-7d7d-b276-cea3b005f79f"),
            PrevHash = [],
            Hash = [1, 2, 3],
            InstitutionId = null,
            SignedHash = [],
            Timestamp = DateTimeOffset.UtcNow
        });

        db.Institutions.Add(new InstitutionEntity
        {
            Id = institutionId,
            BlockId = instBlockId,
            Address = "http://test.com",
            PublicKeyPem = rsaKeyFixture.PublicKey.ExportRSAPublicKey()
        });
        await db.SaveChangesAsync();

        // Configure fake to return valid signature
        mockNodeRepo.CollectSignaturesAsync(
                Arg.Any<BlockBase>(),
                Arg.Any<byte[]>(),
                Arg.Any<byte[]>(),
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var hash = callInfo.ArgAt<byte[]>(1);
                var signedHash =
                    rsaKeyFixture.PrivateKey.SignData(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Task.FromResult<IReadOnlyCollection<CollectedSignature>>(
                [
                    new CollectedSignature(institutionId, signedHash)
                ]);
            });

        var service = new InternalService(
            db,
            mockNodeRepo,
            blockchain,
            crypto,
            rsaKeyFixture.RsaOptionsMonitor
        );

        // Act
        var blockId = Guid.NewGuid();
        var testId = Guid.NewGuid();
        var result = await service.ProposeTestBlockAsync(
            blockId,
            testId,
            institutionId,
            "100",
            "Language Test",
            CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Type).IsEqualTo(BlockType.Test);

        // Verify test was created
        var test = await db.Tests.FindAsync(testId);
        await Assert.That(test).IsNotNull();
        await Assert.That(test!.BlockId).IsEqualTo(result.Id);
        await Assert.That(test.MaxScore).IsEqualTo("100");
        await Assert.That(test.Name).IsEqualTo("Language Test");

        // Verify broadcast
        await mockNodeRepo.Received(1).BroadcastFinalizedBlockAsync(
            "blockchain/blocks/test",
            Arg.Any<AddTestBlockRequest>(),
            Arg.Any<CancellationToken>()
        );
    }

    [Test]
    public async Task ProposeTestResultBlockAsync_CollectsSignaturesAndAddsBlock()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var crypto = scope.ServiceProvider.GetRequiredService<ICryptoService>();
        var blockchain = scope.ServiceProvider.GetRequiredService<BlockchainService>();
        var mockNodeRepo = Substitute.For<INodeRepository>();

        var institutionId = Guid.NewGuid();
        var testId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        // Create institution with block
        var instBlockId = Guid.NewGuid();
        db.Blocks.Add(new BlockEntity
        {
            Id = instBlockId,
            Type = BlockType.Institution,
            PrevId = new Guid("019b6749-15c6-7d7d-b276-cea3b005f79f"),
            PrevHash = [],
            Hash = [1, 2, 3],
            InstitutionId = null,
            SignedHash = [],
            Timestamp = DateTimeOffset.UtcNow
        });

        db.Institutions.Add(new InstitutionEntity
        {
            Id = institutionId,
            BlockId = instBlockId,
            Address = "http://test.com",
            PublicKeyPem = rsaKeyFixture.PublicKey.ExportRSAPublicKey()
        });

        // Create test
        var testBlockId = Guid.NewGuid();
        db.Blocks.Add(new BlockEntity
        {
            Id = testBlockId,
            Type = BlockType.Test,
            PrevId = instBlockId,
            PrevHash = [1, 2, 3],
            Hash = [4, 5, 6],
            InstitutionId = institutionId,
            SignedHash = [],
            Timestamp = DateTimeOffset.UtcNow
        });

        db.Tests.Add(new TestEntity
        {
            Id = testId,
            InstitutionId = institutionId,
            BlockId = testBlockId,
            Name = "Test",
            MaxScore = "100"
        });

        // Create student
        db.Students.Add(new StudentEntity
        {
            Id = studentId,
            Name = "John",
            Surname = "Doe"
        });

        await db.SaveChangesAsync();

        mockNodeRepo.CollectSignaturesAsync(
                Arg.Any<BlockBase>(),
                Arg.Any<byte[]>(),
                Arg.Any<byte[]>(),
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var hash = callInfo.ArgAt<byte[]>(1);
                var signedHash =
                    rsaKeyFixture.PrivateKey.SignData(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Task.FromResult<IReadOnlyCollection<CollectedSignature>>(
                [
                    new CollectedSignature(institutionId, signedHash)
                ]);
            });

        var service = new InternalService(
            db,
            mockNodeRepo,
            blockchain,
            crypto,
            rsaKeyFixture.RsaOptionsMonitor
        );

        // Act
        var blockId = Guid.NewGuid();
        var testResultId = Guid.NewGuid();
        var timestamp = DateTimeOffset.UtcNow;

        var result = await service.ProposeTestResultBlockAsync(
            blockId,
            testResultId,
            testId,
            studentId,
            JsonSerializer.SerializeToDocument(new {listening = 70, speaking = 80}),
            timestamp,
            CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Type).IsEqualTo(BlockType.TestResult);

        // Verify test result was created
        var testResult = await db.TestResults.FindAsync(testResultId);
        await Assert.That(testResult).IsNotNull();
        await Assert.That(testResult!.BlockId).IsEqualTo(result.Id);
        await Assert.That(testResult.Score).IsEqualTo(JsonSerializer.SerializeToDocument(new {listening = 70, speaking = 80}));
        await Assert.That(testResult.StudentId).IsEqualTo(studentId);

        // Verify broadcast
        await mockNodeRepo.Received(1).BroadcastFinalizedBlockAsync(
            "blockchain/blocks/testresult",
            Arg.Any<AddTestResultBlockRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task ProposeInstitutionBlockAsync_PassesCorrectParametersToNodeRepository()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var crypto = scope.ServiceProvider.GetRequiredService<ICryptoService>();
        var blockchain = scope.ServiceProvider.GetRequiredService<BlockchainService>();
        var mockNodeRepo = Substitute.For<INodeRepository>();

        var institutionId = Guid.NewGuid();
        var signerId = Guid.NewGuid();

        // Create signing institution
        var signerBlockId = Guid.NewGuid();
        db.Blocks.Add(new BlockEntity
        {
            Id = signerBlockId,
            Type = BlockType.Institution,
            PrevId = new Guid("019b6749-15c6-7d7d-b276-cea3b005f79f"),
            PrevHash = [],
            Hash = [1, 2, 3],
            InstitutionId = null,
            SignedHash = [],
            Timestamp = DateTimeOffset.UtcNow
        });

        db.Institutions.Add(new InstitutionEntity
        {
            Id = signerId,
            BlockId = signerBlockId,
            Address = "http://signer.com",
            PublicKeyPem = rsaKeyFixture.PublicKey.ExportRSAPublicKey()
        });

        db.Institutions.Add(new InstitutionEntity
        {
            Id = institutionId,
            BlockId = null,
            Address = "http://test.com",
            PublicKeyPem = []
        });
        await db.SaveChangesAsync();

        mockNodeRepo.CollectSignaturesAsync(
                Arg.Any<BlockBase>(),
                Arg.Any<byte[]>(),
                Arg.Any<byte[]>(),
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var hash = callInfo.ArgAt<byte[]>(1);
                var signedHash =
                    rsaKeyFixture.PrivateKey.SignData(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Task.FromResult<IReadOnlyCollection<CollectedSignature>>(
                [
                    new CollectedSignature(signerId, signedHash)
                ]);
            });

        var service = new InternalService(
            db,
            mockNodeRepo,
            blockchain,
            crypto,
            rsaKeyFixture.RsaOptionsMonitor
        );

        // Act
        var call = await service.ProposeInstitutionBlockAsync(
            Guid.NewGuid(),
            institutionId,
            "Test Institution",
            CancellationToken.None
        );

        // Assert
        await mockNodeRepo.Received(1).CollectSignaturesAsync(
            Arg.Any<BlockBase>(),
            Arg.Is<byte[]>(h => h.Length > 0),
            Arg.Is<byte[]>(s => s.Length > 0),
            Arg.Is<string>(p => p.Contains("-----BEGIN RSA PUBLIC KEY-----")),
            Arg.Is<int>(t => t == 1),
            Arg.Any<CancellationToken>());
    }
}