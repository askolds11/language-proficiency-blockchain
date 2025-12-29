using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.Database.Models;
using language_proficiency_blockchain.HashModels;
using language_proficiency_blockchain.HashModels.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace language_proficiency_blockchain.services;

/// <summary>
/// Provides core blockchain operations such as adding blocks, submitting test results,
/// proposing/approving nodes, and retrieving the chain.
/// </summary>
internal class BlockchainService(
    AppDbContext db,
    CryptoService cryptoService
    )
{
    private const int ApprovalThreshold = 2; // minimal demo threshold
    // Serialize block appends within a single process to ensure consistent indexing and prev-hash chaining
    private static readonly SemaphoreSlim BlockAppendLock = new(1, 1);

    /// <summary>
    /// Appends a new block to the chain. This method serializes access to ensure
    /// consistent indexing and previous-hash chaining.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    public async Task AddBlockAsync(BlockBase blockBase, CancellationToken ct = default)
    {
        await BlockAppendLock.WaitAsync(ct);
        try
        {
            //
            //
            // // Fetch the latest block consistently while holding the lock
            var last = await db.Blocks
                .AsNoTracking()
                .Where(e => !db.Blocks.Any(x => x.PrevId == e.Id))
                .FirstOrDefaultAsync(ct);
            // var index = (last?.Index ?? 0) + 1;
            // var prevHash = last?.Hash ?? string.Empty;
            // var timestamp = DateTime.UtcNow;
            //
            // var payload = $"{index}|{type}|{refId}|{dataHash}|{prevHash}|{createdByNodeId}|{timestamp:O}";
            // var hash = CryptoService.ComputeSha256Hash(payload);
            // var signatureBase64 = _crypto.SignHash(payload);
            // var block = new Block
            // {
            //     Type = type,
            //     RefId = refId,
            //     PrevHash = prevHash,
            //     CreatedByNodeId = createdByNodeId,
            //     SignatureBase64 = signatureBase64,
            //     Timestamp = timestamp,
            //     Hash = hash
            // };
            // _db.Blocks.Add(block);
            // await _db.SaveChangesAsync(ct);
            // return block;
        }
        finally
        {
            BlockAppendLock.Release();
        }
    }

    /// <summary>
    /// Submits a test result, verifies the submitter's signature and approval status,
    /// persists the result, and records a corresponding block.
    /// </summary>
    /// <param name="result">Test result to submit.</param>
    /// <param name="signatureBase64">Base64-encoded signature of the canonical result.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The saved <see cref="TestResult"/>.</returns>
    // public async Task<TestResult> SubmitTestResultAsync(TestResult result, string signatureBase64, CancellationToken ct = default)
    // {
    //     // Consensus: signature must match submitting node, and node approved
    //     var node = await _db.Nodes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == result.SubmittedByNodeId, ct);
    //     if (node == null || !node.IsApproved)
    //         throw new InvalidOperationException("Submitting node not approved");
    //
    //     var canonical = CanonicalizeResult(result);
    //     if (!CryptoService.VerifyHash(canonical, signatureBase64, node.PublicKeyPem))
    //         throw new InvalidOperationException("Invalid result signature");
    //
    //     // Compute data hash and store full data separately
    //     result.DataHash = CryptoService.ComputeSha256Hash(canonical);
    //     _db.TestResults.Add(result);
    //     await _db.SaveChangesAsync(ct);
    //
    //     // Add block storing only hash
    //     await AddBlockAsync(BlockType.TestResult, result.Id, result.DataHash, result.SubmittedByNodeId, ct);
    //     return result;
    // }

    /// <summary>
    /// Creates a new node proposal after verifying the node's self-signature that binds
    /// its public key to its declared address.
    /// </summary>
    /// <param name="institution">Node to propose.</param>
    /// <param name="signatureBase64">Base64-encoded self-signature.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The proposed <see cref="Institution"/> (not yet approved).</returns>
    // public async Task<Institution> ProposeNodeAsync(Institution institution, string signatureBase64, CancellationToken ct = default)
    // {
    //     // The node submits its own proposal signed by its key confirming address and key binding
    //     var canonical = $"{institution.PublicKeyPem}|{institution.Address}";
    //     if (!CryptoService.VerifyHash(canonical, signatureBase64, institution.PublicKeyPem))
    //         throw new InvalidOperationException("Invalid self-signature for node proposal");
    //
    //     institution.IsApproved = false;
    //     _db.Nodes.Add(institution);
    //     await _db.SaveChangesAsync(ct);
    //     return institution;
    // }

    /// <summary>
    /// Records an approval for a node by a previously approved approver. Once the
    /// threshold of approvals is reached, the node becomes approved and a block is added.
    /// </summary>
    /// <param name="nodeId">Identifier of the node being approved.</param>
    /// <param name="approverNodeId">Identifier of the approving node.</param>
    /// <param name="signatureBase64">Base64-encoded approval signature.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created <see cref="NodeApproval"/>.</returns>
    // public async Task<NodeApproval> ApproveNodeAsync(Guid nodeId, Guid approverNodeId, string signatureBase64, CancellationToken ct = default)
    // {
    //     var node = await _db.Nodes.FindAsync([nodeId], ct) ?? throw new InvalidOperationException("Node not found");
    //     var approver = await _db.Nodes.FindAsync([approverNodeId], ct) ?? throw new InvalidOperationException("Approver not found");
    //     if (!approver.IsApproved) throw new InvalidOperationException("Approver not approved");
    //
    //     var payload = $"approve|{nodeId}|{approverNodeId}";
    //     if (!CryptoService.VerifyHash(payload, signatureBase64, approver.PublicKeyPem))
    //         throw new InvalidOperationException("Invalid approval signature");
    //
    //     var approval = new NodeApproval
    //     {
    //         InstitutionId = nodeId,
    //         ApproverNodeId = approverNodeId,
    //         SignatureBase64 = signatureBase64
    //     };
    //     _db.NodeApprovals.Add(approval);
    //     await _db.SaveChangesAsync(ct);
    //
    //     // Check threshold
    //     var count = await _db.NodeApprovals.CountAsync(a => a.InstitutionId == nodeId, ct);
    //     if (!node.IsApproved && count >= ApprovalThreshold)
    //     {
    //         node.IsApproved = true;
    //         await _db.SaveChangesAsync(ct);
    //
    //         // Record block that node was added
    //         await AddBlockAsync(BlockType.NodeAdded, nodeId, CryptoService.ComputeSha256Hash(node.PublicKeyPem), approverNodeId, ct);
    //     }
    //     return approval;
    // }

    /// <summary>
    /// Retrieves the entire blockchain ordered by index.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of <see cref="Block"/> entries ordered by index.</returns>
    // public Task<List<Block>> GetChainAsync(CancellationToken ct = default) => _db.Blocks.AsNoTracking().OrderBy(b => b.Index).ToListAsync(ct);

    /// <summary>
    /// Builds a canonical string representation of a <see cref="TestResult"/>
    /// used for hashing and signature verification.
    /// </summary>
    /// <param name="r">Test result.</param>
    /// <returns>Canonical string representation.</returns>
    // private static string CanonicalizeResult(TestResult r)
    //     => $"{r.TestId}|{r.StudentId}|{r.InstitutionId}|{r.Score}|{r.Timestamp:O}|{r.SubmittedByNodeId}";
}
