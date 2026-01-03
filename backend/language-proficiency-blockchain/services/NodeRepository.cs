using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.HashModels.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace language_proficiency_blockchain.services;

/// <summary>
/// Repository implementation for parallel node communication.
/// </summary>
internal sealed class NodeRepository(INodeHttpClient httpClient, AppDbContext db)
    : INodeRepository
{
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
            .Select(i => i.Address)
            .ToListAsync(ct);

        var tasks = institutions.Select(address =>
            httpClient.SendFinalizedBlockAsync(address, endpoint, request, ct));

        await Task.WhenAll(tasks);
    }
}