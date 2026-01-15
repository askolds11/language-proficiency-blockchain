using System.Security.Cryptography;
using System.Text.Json;
using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.Database.Models;
using language_proficiency_blockchain.HashModels;
using language_proficiency_blockchain.HashModels.Interfaces;
using language_proficiency_blockchain.HashModels.v1;
using language_proficiency_blockchain.Options;
using language_proficiency_blockchain.requests.Blockchain;
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
        var hashable = new HashableTestResultV1([], testId, testResultId, score);
        var signatures = await CollectSignaturesAndAddBlockAsync(
            blockId,
            hashable,
            "blockchain/blocks/testresult",
            () => new AddTestResultBlockRequest(blockId, testResultId, testId, studentId, score, timestamp, []),
            ct);

        return await blockchainService.AddTestResultBlockAsync(
            blockId, testResultId, testId, studentId, score, timestamp, signatures, ct);
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