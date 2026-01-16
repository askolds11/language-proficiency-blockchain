using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.HashModels.Interfaces;
using language_proficiency_blockchain.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace language_proficiency_blockchain.services;

/// <summary>
/// Repository implementation for parallel node communication.
/// </summary>
internal sealed class NodeRepository(
    INodeHttpClient httpClient,
    AppDbContext db,
    ICryptoService cryptoService,
    IOptions<NodeOptions> nodeOptions)
    : INodeRepository
{
    private readonly NodeOptions _nodeOptions = nodeOptions.Value;

    public async Task<IReadOnlyCollection<CollectedSignature>> CollectSignaturesAsync(
        BlockBase block,
        byte[] hash,
        byte[] signedHash,
        string proposerPublicKeyPem,
        int threshold,
        CancellationToken ct = default)
    {
        var institutions = await db.Institutions
            .AsNoTracking()
            .Where(i => i.BlockId != null)
            .Select(i => new { i.Id, i.Address })
            .ToListAsync(ct);

        var signatures = new List<CollectedSignature>();
        var lockObj = new Lock();

        var tasks = institutions.Select(async inst =>
        {
            // Check if this institution is the local node
            if (IsLocalNode(inst.Address, inst.Id))
            {
                // Sign locally instead of making HTTP call
                var localSignedHash = cryptoService.SignHash(hash);
                lock (lockObj)
                {
                    if (signatures.Count < threshold)
                    {
                        signatures.Add(new CollectedSignature(inst.Id, localSignedHash));
                    }
                }
                return;
            }

            var response = await httpClient.ProposeBlockAsync(
                inst.Address,
                block,
                hash,
                signedHash,
                proposerPublicKeyPem,
                ct);

            if (response is not null)
            {
                lock (lockObj)
                {
                    if (signatures.Count < threshold)
                    {
                        signatures.Add(new CollectedSignature(
                            inst.Id,
                            Convert.FromBase64String(response.SignedHashBase64)));
                    }
                }
            }
        }).ToList();

        await Task.WhenAll(tasks);

        return signatures;
    }

    public async Task BroadcastFinalizedBlockAsync<TRequest>(
        string endpoint,
        TRequest request,
        CancellationToken ct = default)
    {
        var institutions = await db.Institutions
            .AsNoTracking()
            .Where(i => i.BlockId != null)
            .Select(i => new { i.Id, i.Address })
            .ToListAsync(ct);

        // Skip local node - it already has the block
        var tasks = institutions
            .Where(inst => !IsLocalNode(inst.Address, inst.Id))
            .Select(inst => httpClient.SendFinalizedBlockAsync(inst.Address, endpoint, request, ct));

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Checks if the given institution is the local node.
    /// </summary>
    private bool IsLocalNode(string address, Guid institutionId)
    {
        // Check by institution ID first (more reliable)
        if (_nodeOptions.InstitutionId.HasValue && _nodeOptions.InstitutionId.Value == institutionId)
            return true;

        // Fall back to address comparison
        return string.Equals(
            address.TrimEnd('/'),
            _nodeOptions.Address.TrimEnd('/'),
            StringComparison.OrdinalIgnoreCase);
    }
}