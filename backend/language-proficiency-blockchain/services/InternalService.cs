using System.Security.Cryptography;
using System.Text.Json;
using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.Database.Models;
using language_proficiency_blockchain.HashModels;
using language_proficiency_blockchain.HashModels.Interfaces;
using language_proficiency_blockchain.HashModels.v1;
using language_proficiency_blockchain.Options;
using language_proficiency_blockchain.requests.Blockchain;
using language_proficiency_blockchain.responses.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace language_proficiency_blockchain.services;

internal class InternalService(
    AppDbContext dbContext,
    INodeRepository nodeRepository,
    BlockchainService blockchainService,
    ICryptoService cryptoService,
    IOptionsMonitor<RsaKeyHolder> rsaKeyHolder
)
{
    public async Task AssignRoleAsync(Guid userId, UserRole role)
    {
        var userExists = await dbContext.Users.AnyAsync(x => x.Id == userId);
        if (!userExists)
        {
            throw new InvalidOperationException("User not found");
        }

        var roleExists = await dbContext.UserRoles.AnyAsync(x => x.UserId == userId && x.Role == role);
        if (roleExists)
        {
            return; // Role already assigned
        }

        var roleAssignment = new UserRoleAssociation
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Role = role,
            AssignedAt = DateTime.UtcNow
        };

        dbContext.UserRoles.Add(roleAssignment);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddInstitution(Guid id, string name, string address, string publicKeyPem)
    {
        var exists = await dbContext.Institutions.AnyAsync(x => x.Id == id);

        if (exists)
        {
            throw new Exception("Institution already exists");
        }
        
        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);
        var publicKeyBytes = rsa.ExportRSAPublicKey();
        
        var institution = new InstitutionEntity
        {
            Id = id,
            BlockId = null,
            Address = address,
            PublicKeyPem = publicKeyBytes,
        };
        
        dbContext.Institutions.Add(institution);

        await dbContext.SaveChangesAsync();
    }

    public async Task AddStudent(Guid id, string? name, string? surname)
    {
        var exists = await dbContext.Students.AnyAsync(x => x.Id == id);
        if (exists)
        {
            throw new Exception("Student already exists");
        }

        var student = new StudentEntity
        {
            Id = id,
            Name = name,
            Surname = surname
        };

        dbContext.Students.Add(student);
        await dbContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// Proposes a new institution block to the network, collects signatures, and adds it to the blockchain.
    /// </summary>
    public async Task<BlockEntity> ProposeInstitutionBlockAsync(
        Guid blockId,
        Guid institutionId,
        string institutionName,
        CancellationToken ct = default)
    {
        var institution = await dbContext.Institutions.FirstOrDefaultAsync(x => x.Id == institutionId, ct)
            ?? throw new InvalidOperationException("Institution must exist before adding its block.");

        if (institution.BlockId is not null)
        {
            throw new InvalidOperationException("Institution already has a block.");
        }

        var hashable = new HashableInstitutionV1([], institutionId, institutionName);
        var signatures = await CollectSignaturesAndAddBlockAsync(
            blockId,
            hashable,
            "blockchain/blocks/institution",
            () => new AddInstitutionBlockRequest(blockId, institutionId, institutionName, []),
            ct);

        return await blockchainService.AddInstitutionBlockAsync(blockId, institutionId, institutionName, signatures, ct);
    }

    /// <summary>
    /// Proposes a new test block to the network, collects signatures, and adds it to the blockchain.
    /// </summary>
    public async Task<BlockEntity> ProposeTestBlockAsync(
        Guid blockId,
        Guid testId,
        Guid institutionId,
        string maxScore,
        string? name,
        CancellationToken ct = default)
    {
        var hashable = new HashableTestV1([], institutionId, testId, maxScore);
        var signatures = await CollectSignaturesAndAddBlockAsync(
            blockId,
            hashable,
            "blockchain/blocks/test",
            () => new AddTestBlockRequest(blockId, testId, institutionId, maxScore, name, []),
            ct);

        return await blockchainService.AddTestBlockAsync(blockId, testId, institutionId, maxScore, name, signatures, ct);
    }

    /// <summary>
    /// Proposes a new test result block to the network, collects signatures, and adds it to the blockchain.
    /// </summary>
    public async Task<BlockEntity> ProposeTestResultBlockAsync(
        Guid blockId,
        Guid testResultId,
        Guid testId,
        Guid studentId,
        JsonDocument score,
        DateTimeOffset timestamp,
        CancellationToken ct = default)
    {
        // Normalize the JSON to ensure consistent formatting for hashing
        var normalizedScore = NormalizeJsonDocument(score);
        
        var hashable = new HashableTestResultV1([], testId, testResultId, normalizedScore);
        var signatures = await CollectSignaturesAndAddBlockAsync(
            blockId,
            hashable,
            "blockchain/blocks/testresult",
            () => new AddTestResultBlockRequest(blockId, testResultId, testId, studentId, normalizedScore, timestamp, []),
            ct);

        return await blockchainService.AddTestResultBlockAsync(
            blockId, testResultId, testId, studentId, normalizedScore, timestamp, signatures, ct);
    }

    /// <summary>
    /// Gets all test results for a user (via their linked student record).
    /// </summary>
    public async Task<IReadOnlyList<TestResultWithTestResponse>> GetUserTestResultsAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user?.StudentId == null)
            return [];

        var testResults = await dbContext.TestResults
            .Include(tr => tr.TestEntity)
            .Where(tr => tr.StudentId == user.StudentId)
            .Join(
                dbContext.Blocks,
                tr => tr.BlockId,
                b => b.Id,
                (tr, b) => new { TestResult = tr, Block = b })
            .ToListAsync(ct);

        return testResults.Select(x => new TestResultWithTestResponse(
            TestResultId: x.TestResult.Id,
            TestId: x.TestResult.TestId,
            TestName: x.TestResult.TestEntity.Name,
            TestMaxScore: x.TestResult.TestEntity.MaxScore,
            InstitutionId: x.TestResult.TestEntity.InstitutionId,
            Score: x.TestResult.Score,
            Timestamp: x.TestResult.Timestamp,
            BlockHash: Convert.ToHexString(x.Block.Hash).ToLowerInvariant(),
            PrevBlockHash: Convert.ToHexString(x.Block.PrevHash).ToLowerInvariant()
        )).ToList();
    }

    /// <summary>
    /// Verifies that the provided test result data matches the stored hash in the blockchain.
    /// </summary>
    public async Task<VerifyTestResultResponse?> VerifyTestResultAsync(
        Guid testResultId,
        Guid testId,
        JsonDocument score,
        string prevHashHex,
        CancellationToken ct = default)
    {
        var testResult = await dbContext.TestResults
            .Include(tr => tr.TestEntity)
            .FirstOrDefaultAsync(tr => tr.Id == testResultId, ct);

        if (testResult == null)
            return null;

        var block = await dbContext.Blocks
            .FirstOrDefaultAsync(b => b.Id == testResult.BlockId, ct);

        if (block == null)
            return null;

        // Convert provided hex to bytes
        var prevHash = Convert.FromHexString(prevHashHex);

        // Normalize the JSON and recreate the hashable object with provided data
        var normalizedScore = NormalizeJsonDocument(score);
        var hashable = new HashableTestResultV1(prevHash, testId, testResultId, normalizedScore);
        var computedHash = Hasher.HashBlock(hashable);
        
        var storedHashHex = Convert.ToHexString(block.Hash).ToLowerInvariant();
        var computedHashHex = Convert.ToHexString(computedHash).ToLowerInvariant();

        return new VerifyTestResultResponse(
            IsValid: storedHashHex == computedHashHex,
            StoredHash: storedHashHex,
            ComputedHash: computedHashHex
        );
    }

    /// <summary>
    /// Creates a shareable code for a test result.
    /// </summary>
    public async Task<ShareCodeResponse> CreateShareCodeAsync(
        Guid testResultId,
        Guid userId,
        DateTimeOffset expiresAt,
        CancellationToken ct = default)
    {
        if (expiresAt <= DateTimeOffset.UtcNow)
            throw new ArgumentException("Expiration date must be in the future", nameof(expiresAt));

        // Get user's student ID
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user?.StudentId == null)
            throw new InvalidOperationException("User is not linked to a student record");

        // Verify the test result belongs to the user's student
        var testResult = await dbContext.TestResults
            .FirstOrDefaultAsync(tr => tr.Id == testResultId && tr.StudentId == user.StudentId, ct);
        
        if (testResult == null)
            throw new InvalidOperationException("Test result not found or does not belong to you");

        var code = GenerateShareCode();
        
        // Ensure uniqueness (extremely unlikely to collide, but just in case)
        while (await dbContext.ShareCodes.AnyAsync(sc => sc.Code == code, ct))
        {
            code = GenerateShareCode();
        }

        var shareCode = new ShareCodeEntity
        {
            Id = Guid.NewGuid(),
            Code = code,
            TestResultId = testResultId,
            CreatedByUserId = userId,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = expiresAt,
            IsRevoked = false
        };

        dbContext.ShareCodes.Add(shareCode);
        await dbContext.SaveChangesAsync(ct);

        return new ShareCodeResponse(code, expiresAt);
    }

    /// <summary>
    /// Retrieves a shared test result by share code.
    /// </summary>
    public async Task<SharedTestResultResponse?> GetSharedTestResultAsync(
        string code,
        CancellationToken ct = default)
    {
        var shareCode = await dbContext.ShareCodes
            .Include(sc => sc.TestResult)
                .ThenInclude(tr => tr!.TestEntity)
            .FirstOrDefaultAsync(sc => sc.Code == code, ct);

        if (shareCode == null || shareCode.IsRevoked || shareCode.ExpiresAt < DateTimeOffset.UtcNow)
            return null;

        var testResult = shareCode.TestResult;
        if (testResult == null)
            return null;

        var block = await dbContext.Blocks
            .Include(b => b.PrevBlock)
            .FirstOrDefaultAsync(b => b.Id == testResult.BlockId, ct);

        if (block == null)
            return null;

        return new SharedTestResultResponse(
            TestResultId: testResult.Id,
            TestId: testResult.TestId,
            StudentId: testResult.StudentId,
            Score: testResult.Score,
            Timestamp: testResult.Timestamp,
            TestName: testResult.TestEntity.Name,
            InstitutionId: testResult.TestEntity.InstitutionId,
            BlockHash: Convert.ToHexString(block.Hash).ToLowerInvariant(),
            PrevBlockHash: Convert.ToHexString(block.PrevHash).ToLowerInvariant()
        );
    }

    /// <summary>
    /// Revokes a share code so it can no longer be used.
    /// </summary>
    public async Task<bool> RevokeShareCodeAsync(
        string code,
        Guid userId,
        CancellationToken ct = default)
    {
        var shareCode = await dbContext.ShareCodes
            .FirstOrDefaultAsync(sc => sc.Code == code && sc.CreatedByUserId == userId, ct);

        if (shareCode == null)
            return false;

        shareCode.IsRevoked = true;
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Normalizes a JsonDocument to ensure consistent formatting for hashing.
    /// Re-parses and re-serializes using the same options as HasherV1.
    /// </summary>
    private static JsonDocument NormalizeJsonDocument(JsonDocument document)
    {
        // Serialize and re-parse to normalize the JSON format
        var json = HasherV1.Serialize(document.RootElement);
        return JsonDocument.Parse(json);
    }

    /// <summary>
    /// Generates a short, readable share code using Base32-like characters (no ambiguous chars).
    /// </summary>
    private static string GenerateShareCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Span<char> result = stackalloc char[8];
        
        for (var i = 0; i < 8; i++)
        {
            result[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
        }
        
        return new string(result);
    }

    private async Task<IReadOnlyCollection<BlockchainService.BlockSignature>> CollectSignaturesAndAddBlockAsync<TRequest>(
        Guid blockId,
        BlockBase hashable,
        string broadcastEndpoint,
        Func<TRequest> createBroadcastRequest,
        CancellationToken ct)
    {
        var hash = Hasher.HashBlock(hashable);
        var signedHash = cryptoService.SignHash(hash);
        var publicKeyPem = rsaKeyHolder.CurrentValue.PublicKey.ExportRSAPublicKeyPem();

        var totalInstitutions = await dbContext.Institutions.CountAsync(i => i.BlockId != null, ct);
        
        // Bootstrap case: no institutions with blocks yet - no signatures needed
        if (totalInstitutions == 0)
        {
            return [];
        }
        
        var threshold = Math.Max(1, (totalInstitutions + 1) / 2);

        var collectedSignatures = await nodeRepository.CollectSignaturesAsync(
            hashable,
            hash,
            signedHash,
            publicKeyPem,
            threshold,
            ct);

        if (collectedSignatures.Count < threshold)
        {
            throw new InvalidOperationException(
                $"Failed to collect enough signatures. Got {collectedSignatures.Count}/{threshold}.");
        }

        var signatures = collectedSignatures
            .Select(s => new BlockchainService.BlockSignature(s.InstitutionId, s.SignedHash))
            .ToList();

        // Broadcast to all institutions after successful addition
        await nodeRepository.BroadcastFinalizedBlockAsync(broadcastEndpoint, createBroadcastRequest(), ct);

        return signatures;
    }
}