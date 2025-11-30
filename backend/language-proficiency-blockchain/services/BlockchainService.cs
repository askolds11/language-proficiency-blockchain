using language_proficiency_blockchain.data;
using language_proficiency_blockchain.models;
using Microsoft.EntityFrameworkCore;

namespace language_proficiency_blockchain.services;

/// <summary>
/// Provides core blockchain operations such as adding blocks, submitting test results,
/// proposing/approving nodes, and retrieving the chain.
/// </summary>
public class BlockchainService
{
    private readonly AppDbContext _db;
    private readonly CryptoService _crypto;
    private const int ApprovalThreshold = 2; // minimal demo threshold
    // Serialize block appends within a single process to ensure consistent indexing and prev-hash chaining
    private static readonly SemaphoreSlim BlockAppendLock = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="BlockchainService"/> class.
    /// </summary>
    /// <param name="db">Application database context.</param>
    /// <param name="crypto">Crypto service used to sign blocks.</param>
    public BlockchainService(AppDbContext db, CryptoService crypto)
    {
        _db = db;
        _crypto = crypto;
    }

    /// <summary>
    /// Appends a new block to the chain. This method serializes access to ensure
    /// consistent indexing and previous-hash chaining.
    /// </summary>
    /// <param name="type">Block type.</param>
    /// <param name="refId">Reference identifier associated with the block.</param>
    /// <param name="dataHash">Hash of the block payload data.</param>
    /// <param name="createdByNodeId">Identifier of the node creating the block.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created <see cref="Block"/>.</returns>
    public async Task<Block> AddBlockAsync(BlockType type, Guid refId, string dataHash, Guid createdByNodeId, CancellationToken ct = default)
    {
        await BlockAppendLock.WaitAsync(ct);
        try
        {
            // Fetch the latest block consistently while holding the lock
            var last = await _db.Blocks.AsNoTracking().OrderByDescending(b => b.Index).FirstOrDefaultAsync(ct);
            var index = (last?.Index ?? 0) + 1;
            var prevHash = last?.Hash ?? string.Empty;
            var timestamp = DateTime.UtcNow;

            var payload = $"{index}|{type}|{refId}|{dataHash}|{prevHash}|{createdByNodeId}|{timestamp:O}";
            var hash = CryptoService.ComputeSha256Hex(payload);
            var signatureBase64 = _crypto.SignToBase64(payload);
            var block = new Block
            {
                Index = index,
                Type = type,
                RefId = refId,
                DataHash = dataHash,
                PrevHash = prevHash,
                CreatedByNodeId = createdByNodeId,
                SignatureBase64 = signatureBase64,
                Timestamp = timestamp,
                Hash = hash
            };
            _db.Blocks.Add(block);
            await _db.SaveChangesAsync(ct);
            return block;
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
    public async Task<TestResult> SubmitTestResultAsync(TestResult result, string signatureBase64, CancellationToken ct = default)
    {
        // Consensus: signature must match submitting node, and node approved
        var node = await _db.Nodes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == result.SubmittedByNodeId, ct);
        if (node == null || !node.IsApproved)
            throw new InvalidOperationException("Submitting node not approved");

        var canonical = CanonicalizeResult(result);
        if (!CryptoService.VerifyWithPublicPem(canonical, signatureBase64, node.PublicKeyPem))
            throw new InvalidOperationException("Invalid result signature");

        // Compute data hash and store full data separately
        result.DataHash = CryptoService.ComputeSha256Hex(canonical);
        _db.TestResults.Add(result);
        await _db.SaveChangesAsync(ct);

        // Add block storing only hash
        await AddBlockAsync(BlockType.TestResult, result.Id, result.DataHash, result.SubmittedByNodeId, ct);
        return result;
    }

    /// <summary>
    /// Creates a new node proposal after verifying the node's self-signature that binds
    /// its public key to its declared address.
    /// </summary>
    /// <param name="node">Node to propose.</param>
    /// <param name="signatureBase64">Base64-encoded self-signature.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The proposed <see cref="Node"/> (not yet approved).</returns>
    public async Task<Node> ProposeNodeAsync(Node node, string signatureBase64, CancellationToken ct = default)
    {
        // The node submits its own proposal signed by its key confirming address and key binding
        var canonical = $"{node.PublicKeyPem}|{node.Address}";
        if (!CryptoService.VerifyWithPublicPem(canonical, signatureBase64, node.PublicKeyPem))
            throw new InvalidOperationException("Invalid self-signature for node proposal");

        node.IsApproved = false;
        _db.Nodes.Add(node);
        await _db.SaveChangesAsync(ct);
        return node;
    }

    /// <summary>
    /// Records an approval for a node by a previously approved approver. Once the
    /// threshold of approvals is reached, the node becomes approved and a block is added.
    /// </summary>
    /// <param name="nodeId">Identifier of the node being approved.</param>
    /// <param name="approverNodeId">Identifier of the approving node.</param>
    /// <param name="signatureBase64">Base64-encoded approval signature.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created <see cref="NodeApproval"/>.</returns>
    public async Task<NodeApproval> ApproveNodeAsync(Guid nodeId, Guid approverNodeId, string signatureBase64, CancellationToken ct = default)
    {
        var node = await _db.Nodes.FindAsync([nodeId], ct) ?? throw new InvalidOperationException("Node not found");
        var approver = await _db.Nodes.FindAsync([approverNodeId], ct) ?? throw new InvalidOperationException("Approver not found");
        if (!approver.IsApproved) throw new InvalidOperationException("Approver not approved");

        var payload = $"approve|{nodeId}|{approverNodeId}";
        if (!CryptoService.VerifyWithPublicPem(payload, signatureBase64, approver.PublicKeyPem))
            throw new InvalidOperationException("Invalid approval signature");

        var approval = new NodeApproval
        {
            NodeId = nodeId,
            ApproverNodeId = approverNodeId,
            SignatureBase64 = signatureBase64
        };
        _db.NodeApprovals.Add(approval);
        await _db.SaveChangesAsync(ct);

        // Check threshold
        var count = await _db.NodeApprovals.CountAsync(a => a.NodeId == nodeId, ct);
        if (!node.IsApproved && count >= ApprovalThreshold)
        {
            node.IsApproved = true;
            await _db.SaveChangesAsync(ct);

            // Record block that node was added
            await AddBlockAsync(BlockType.NodeAdded, nodeId, CryptoService.ComputeSha256Hex(node.PublicKeyPem), approverNodeId, ct);
        }
        return approval;
    }

    /// <summary>
    /// Retrieves the entire blockchain ordered by index.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of <see cref="Block"/> entries ordered by index.</returns>
    public Task<List<Block>> GetChainAsync(CancellationToken ct = default) => _db.Blocks.AsNoTracking().OrderBy(b => b.Index).ToListAsync(ct);

    /// <summary>
    /// Builds a canonical string representation of a <see cref="TestResult"/>
    /// used for hashing and signature verification.
    /// </summary>
    /// <param name="r">Test result.</param>
    /// <returns>Canonical string representation.</returns>
    private static string CanonicalizeResult(TestResult r)
        => $"{r.TestId}|{r.StudentId}|{r.InstitutionId}|{r.Score}|{r.Timestamp:O}|{r.SubmittedByNodeId}";
}
