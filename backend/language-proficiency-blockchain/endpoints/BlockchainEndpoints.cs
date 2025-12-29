using JetBrains.Annotations;
using language_proficiency_blockchain.requests;
using language_proficiency_blockchain.responses;
using language_proficiency_blockchain.services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace language_proficiency_blockchain.endpoints;

/// <summary>
/// HTTP endpoints for blockchain-related operations such as node management,
/// chain retrieval and submitting test results.
/// </summary>
[PublicAPI]
public class BlockchainEndpoints : IEndpoint
{
    /// <summary>
    /// Registers all blockchain endpoints under the <c>/blockchain</c> route group.
    /// </summary>
    /// <param name="builder">Endpoint route builder to map routes on.</param>
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        // var group = builder.MapGroup("blockchain").WithTags("Blockchain");

        // group.MapPost("nodes", ProposeNode);
        // group.MapPost("nodes/{id:guid}/approve", ApproveNode);
        // group.MapGet("nodes", ListNodes);
        // group.MapGet("chain", GetChain);
        // group.MapGet("node-info", GetLocalNodeInfo);

        // group.MapPost("results", SubmitResult);
        // group.MapGet("results/{id:guid}", GetResult);
    }

    /// <summary>
    /// Proposes a new node to join the network. The proposal must be signed by the node's key.
    /// </summary>
    /// <param name="chain">Blockchain service.</param>
    /// <param name="req">Proposal payload including public key, address and signature.</param>
    /// <returns>
    /// 200 OK with the created node when proposal is valid; 400 Bad Request otherwise.
    /// </returns>
    // public static async Task<Results<Ok<Institution>, BadRequest<string>>> ProposeNode(
    //     [FromServices] BlockchainService chain,
    //     [FromBody] ProposeNodeRequest req)
    // {
    //     try
    //     {
    //         var node = new Institution { PublicKeyPem = req.PublicKeyPem, Address = req.Address };
    //         var created = await chain.ProposeNodeAsync(node, req.SignatureBase64);
    //         return TypedResults.Ok(created);
    //     }
    //     catch (Exception ex)
    //     {
    //         return TypedResults.BadRequest(ex.Message);
    //     }
    // }

    /// <summary>
    /// Approves a node using the approver's signature. Once the approval threshold is met,
    /// the node becomes approved and a block is recorded.
    /// </summary>
    /// <param name="chain">Blockchain service.</param>
    /// <param name="id">Identifier of the node being approved.</param>
    /// <param name="req">Approval request containing approver id and signature.</param>
    /// <returns>
    /// 200 OK with the created approval; 400 Bad Request if validation fails.
    /// </returns>
    // public static async Task<Results<Ok<NodeApproval>, BadRequest<string>>> ApproveNode(
    //     [FromServices] BlockchainService chain,
    //     [FromRoute] Guid id,
    //     [FromBody] ApproveNodeRequest req)
    // {
    //     try
    //     {
    //         var approval = await chain.ApproveNodeAsync(id, req.ApproverNodeId, req.SignatureBase64);
    //         return TypedResults.Ok(approval);
    //     }
    //     catch (Exception ex)
    //     {
    //         return TypedResults.BadRequest(ex.Message);
    //     }
    // }

    /// <summary>
    /// Lists all nodes known to the system.
    /// </summary>
    /// <param name="db">Application database context.</param>
    /// <returns>200 OK with the list of nodes.</returns>
    // public static async Task<Ok<List<Institution>>> ListNodes([FromServices] AppDbContext db)
    // {
    //     var nodes = await db.Nodes.AsNoTracking().ToListAsync();
    //     return TypedResults.Ok(nodes);
    // }

    /// <summary>
    /// Returns the full blockchain ordered by block index.
    /// </summary>
    /// <param name="chain">Blockchain service.</param>
    /// <returns>200 OK with the list of blocks.</returns>
    // public static async Task<Ok<List<Block>>> GetChain([FromServices] BlockchainService chain)
    // {
    //     var blocks = await chain.GetChainAsync();
    //     return TypedResults.Ok(blocks);
    // }

    /// <summary>
    /// Returns local node public information, currently the public key in PEM format.
    /// </summary>
    /// <param name="crypto">Crypto service providing the local public key.</param>
    /// <returns>200 OK with a dictionary containing the public key.</returns>
    // public static Ok<Dictionary<string, string>> GetLocalNodeInfo([FromServices] CryptoService crypto)
    //     => TypedResults.Ok(new Dictionary<string, string> { { "publicKeyPem", crypto.PublicKeyPem } });

    /// <summary>
    /// Submits a test result to be recorded on-chain. The payload must be signed by the submitting node.
    /// </summary>
    /// <param name="chain">Blockchain service.</param>
    /// <param name="req">Submission request with result details and signature.</param>
    /// <returns>
    /// 200 OK with the saved result; 400 Bad Request if validation fails.
    /// </returns>
    // public static async Task<Results<Ok<TestResult>, BadRequest<string>>> SubmitResult(
    //     [FromServices] BlockchainService chain,
    //     [FromBody] SubmitResultRequest req)
    // {
    //     try
    //     {
    //         var result = new TestResult
    //         {
    //             TestId = req.TestId,
    //             StudentId = req.StudentId,
    //             InstitutionId = req.InstitutionId,
    //             Score = req.Score,
    //             SubmittedByNodeId = req.SubmittedByNodeId,
    //             Timestamp = req.Timestamp
    //         };
    //         var saved = await chain.SubmitTestResultAsync(result, req.SignatureBase64);
    //         return TypedResults.Ok(saved);
    //     }
    //     catch (Exception ex)
    //     {
    //         return TypedResults.BadRequest(ex.Message);
    //     }
    // }

    /// <summary>
    /// Gets a previously submitted test result by its identifier.
    /// </summary>
    /// <param name="db">Application database context.</param>
    /// <param name="id">Identifier of the test result.</param>
    /// <returns>200 OK with the result if found; 404 Not Found otherwise.</returns>
    // public static async Task<Results<Ok<TestResult>, NotFound>> GetResult([FromServices] AppDbContext db, [FromRoute] Guid id)
    // {
    //     var r = await db.TestResults.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    //     if (r == null) return TypedResults.NotFound();
    //     return TypedResults.Ok(r);
    // }
}
