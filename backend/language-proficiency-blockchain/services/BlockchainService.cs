using System.Text.Json;
using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.Database.Models;
using language_proficiency_blockchain.HashModels;
using language_proficiency_blockchain.HashModels.Interfaces;
using language_proficiency_blockchain.HashModels.v1;
using Microsoft.EntityFrameworkCore;

namespace language_proficiency_blockchain.services;

/// <summary>
/// Core blockchain operations: block proposal validation and typed block append helpers.
/// </summary>
internal sealed class BlockchainService(AppDbContext db, ICryptoService cryptoService)
{
    private static readonly SemaphoreSlim BlockAppendLock = new(1, 1);

    public record BlockSignature(Guid InstitutionId, byte[] SignedHash);

    /// <summary>
    /// Validates a proposed block by recomputing its hash with PrevHash cleared, and verifying the provided signature.
    /// If valid, signs the hash with the local private key and returns the signed hash.
    /// </summary>
    public byte[] ProposeBlockAsync(BlockBase proposedBlock, byte[] providedHash, byte[] providedSignedHash,
        byte[] proposerPublicKey)
    {
        var canonical = StripPrevHash(proposedBlock);
        var expectedHash = Hasher.HashBlock(canonical);

        if (!providedHash.SequenceEqual(expectedHash))
        {
            throw new InvalidOperationException("Hash mismatch.");
        }

        if (!CryptoService.VerifyHash(providedHash, providedSignedHash, proposerPublicKey))
        {
            throw new InvalidOperationException("Signature verification failed.");
        }

        return cryptoService.SignHash(expectedHash);
    }

    public async Task<BlockEntity> AddInstitutionBlockAsync(
        Guid blockId,
        Guid institutionId,
        string institutionName,
        IReadOnlyCollection<BlockSignature> signatures,
        CancellationToken ct = default)
    {
        await BlockAppendLock.WaitAsync(ct);
        try
        {
            var institution = await db.Institutions.FirstOrDefaultAsync(x => x.Id == institutionId, ct)
                              ?? throw new InvalidOperationException("Institution must exist before adding its block.");
            if (institution.BlockId is not null)
            {
                throw new InvalidOperationException("Institution already has a block.");
            }

            var (prevId, prevHash) = await GetTailAsync(ct);
            var validateHashable = new HashableInstitutionV1([], institutionId, institutionName);
            var validateHash = Hasher.HashBlock(validateHashable);
            var authorId = await ValidateQuorumAsync(validateHash, signatures, ct);
            var hashable = new HashableInstitutionV1(prevHash, institutionId, institutionName);
            var hash = Hasher.HashBlock(hashable);
            var block = await PersistBlockAsync(blockId, BlockType.Institution, prevId, prevHash, authorId, hash, ct);

            institution.BlockId = block.Id;
            await db.SaveChangesAsync(ct);
            return block;
        }
        finally
        {
            BlockAppendLock.Release();
        }
    }

    public async Task<BlockEntity> AddTestBlockAsync(
        Guid blockId,
        Guid testId,
        Guid institutionId,
        string maxScore,
        string? name,
        IReadOnlyCollection<BlockSignature> signatures,
        CancellationToken ct = default)
    {
        await BlockAppendLock.WaitAsync(ct);
        try
        {
            var institutionExists = await db.Institutions.AnyAsync(x => x.Id == institutionId, ct);
            if (!institutionExists)
            {
                throw new InvalidOperationException("Institution must exist before adding a test.");
            }

            var (prevId, prevHash) = await GetTailAsync(ct);
            var validateHashable = new HashableTestV1([], institutionId, testId, maxScore);
            var validateHash = Hasher.HashBlock(validateHashable);
            var authorId = await ValidateQuorumAsync(validateHash, signatures, ct);
            var hashable = new HashableTestV1(prevHash, institutionId, testId, maxScore);
            var hash = Hasher.HashBlock(hashable);
            var block = await PersistBlockAsync(blockId, BlockType.Test, prevId, prevHash, authorId, hash, ct);

            var test = new TestEntity
            {
                Id = testId,
                InstitutionId = institutionId,
                BlockId = block.Id,
                Name = name,
                MaxScore = maxScore
            };

            db.Tests.Add(test);
            await db.SaveChangesAsync(ct);
            return block;
        }
        finally
        {
            BlockAppendLock.Release();
        }
    }

    public async Task<BlockEntity> AddTestResultBlockAsync(
        Guid blockId,
        Guid testResultId,
        Guid testId,
        Guid studentId,
        JsonDocument score,
        DateTimeOffset timestamp,
        IReadOnlyCollection<BlockSignature> signatures,
        CancellationToken ct = default)
    {
        await BlockAppendLock.WaitAsync(ct);
        try
        {
            var test = await db.Tests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == testId, ct)
                       ?? throw new InvalidOperationException("Test must exist before adding a result.");

            var studentExists = await db.Students.AsNoTracking().AnyAsync(x => x.Id == studentId, ct);
            if (!studentExists)
            {
                db.Students.Add(new StudentEntity { Id = studentId, Name = null, Surname = null });
            }

            var (prevId, prevHash) = await GetTailAsync(ct);
            var validateHashable = new HashableTestResultV1([], testId, testResultId, score);
            var validateHash = Hasher.HashBlock(validateHashable);
            var authorId = await ValidateQuorumAsync(validateHash, signatures, ct);
            var hashable = new HashableTestResultV1(prevHash, testId, testResultId, score);
            var hash = Hasher.HashBlock(hashable);
            var block = await PersistBlockAsync(blockId, BlockType.TestResult, prevId, prevHash, authorId, hash, ct);
            var result = new TestResultEntity
            {
                Id = testResultId,
                TestId = testId,
                BlockId = block.Id,
                StudentId = studentId,
                Score = score,
                Timestamp = timestamp
            };

            db.TestResults.Add(result);
            await db.SaveChangesAsync(ct);
            return block;
        }
        finally
        {
            BlockAppendLock.Release();
        }
    }

    private async Task<(Guid PrevId, byte[] PrevHash)> GetTailAsync(CancellationToken ct)
    {
        var tail = await db.Blocks
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.Id != e.PrevId && !db.Blocks.Any(x => x.PrevId == e.Id),
                ct);

        if (tail is not null)
        {
            return (tail.Id, tail.Hash);
        }

        var genesis = await db.Blocks
                          .AsNoTracking()
                          .FirstOrDefaultAsync(e => e.Id == e.PrevId, ct)
                      ?? throw new InvalidOperationException("Blockchain is not initialized.");

        return (genesis.Id, genesis.Hash);
    }

    private async Task<Guid> ValidateQuorumAsync(byte[] hash, IReadOnlyCollection<BlockSignature> signatures,
        CancellationToken ct)
    {
        var totalInstitutions = await db.Institutions.CountAsync(ct);
        if (totalInstitutions == 0)
        {
            throw new InvalidOperationException("No institutions available to sign.");
        }

        var quorum = (totalInstitutions + 1) / 2; // at least half, rounded up
        var signerIds = signatures.Select(s => s.InstitutionId).Distinct().ToArray();

        var institutions = await db.Institutions
            .Where(i => signerIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, ct);

        var validSigners = new List<Guid>();
        foreach (var sig in signatures)
        {
            if (!institutions.TryGetValue(sig.InstitutionId, out var inst))
            {
                continue;
            }

            var isValid = CryptoService.VerifyHash(hash, sig.SignedHash, inst.PublicKeyPem);
            if (isValid && !validSigners.Contains(sig.InstitutionId))
            {
                validSigners.Add(sig.InstitutionId);
            }
        }

        if (validSigners.Count < quorum)
        {
            throw new InvalidOperationException($"Insufficient signatures: {validSigners.Count}/{quorum} valid.");
        }

        return validSigners[0];
    }

    private async Task<BlockEntity> PersistBlockAsync(Guid blockId, BlockType type, Guid prevId, byte[] prevHash,
        Guid? institutionId, byte[] hash, CancellationToken ct)
    {
        var signedHash = cryptoService.SignHash(hash);

        var block = new BlockEntity
        {
            Id = blockId,
            Type = type,
            PrevId = prevId,
            PrevHash = prevHash,
            Hash = hash,
            InstitutionId = institutionId,
            SignedHash = signedHash,
            Timestamp = DateTimeOffset.UtcNow
        };

        db.Blocks.Add(block);
        await db.SaveChangesAsync(ct);
        return block;
    }

    private static BlockBase StripPrevHash(BlockBase blockBase) => blockBase switch
    {
        HashableInstitutionV1 b => b with { PrevHash = [] },
        HashableTestV1 b => b with { PrevHash = [] },
        HashableTestResultV1 b => b with { PrevHash = [] },
        _ => throw new InvalidOperationException("Unsupported block type.")
    };
}