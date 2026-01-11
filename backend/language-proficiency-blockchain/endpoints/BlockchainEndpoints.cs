using JetBrains.Annotations;
using language_proficiency_blockchain.Database.Models;
using language_proficiency_blockchain.requests.Blockchain;
using language_proficiency_blockchain.responses.Blockchain;
using language_proficiency_blockchain.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace language_proficiency_blockchain.endpoints;

/// <summary>
/// HTTP endpoints for blockchain-related operations such as adding blocks.
/// </summary>
[PublicAPI]
[Authorize]
public class BlockchainEndpoints : IEndpoint
{
    /// <summary>
    /// Registers all blockchain endpoints under the <c>/blockchain</c> route group.
    /// </summary>
    /// <param name="builder">Endpoint route builder to map routes on.</param>
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("blockchain").WithTags("Blockchain").RequireAuthorization();

        group.MapPost("blocks/institution", AddInstitutionBlock)
            .RequireAuthorization(language_proficiency_blockchain.Authorization.AuthorizationPolicies.OperatorOnly);
        group.MapPost("blocks/test", AddTestBlock)
            .RequireAuthorization(language_proficiency_blockchain.Authorization.AuthorizationPolicies.OperatorOnly);
        group.MapPost("blocks/testresult", AddTestResultBlock)
            .RequireAuthorization(language_proficiency_blockchain.Authorization.AuthorizationPolicies.VerificatorOrOperator);
        group.MapPost("blocks/propose", ProposeBlock)
            .RequireAuthorization(language_proficiency_blockchain.Authorization.AuthorizationPolicies.VerificatorOrOperator);
    }

    internal static async Task<Results<Ok<BlockEntity>, BadRequest<string>>> AddInstitutionBlock(
        [FromServices] BlockchainService chain,
        [FromBody] AddInstitutionBlockRequest req,
        CancellationToken ct)
    {
        try
        {
            var signatures = req.Signatures
                .Select(s =>
                    new BlockchainService.BlockSignature(s.InstitutionId, Convert.FromBase64String(s.SignedHashBase64)))
                .ToList();

            var block = await chain.AddInstitutionBlockAsync(req.BlockId, req.InstitutionId, req.InstitutionName,
                signatures, ct);
            return TypedResults.Ok(block);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    internal static async Task<Results<Ok<BlockEntity>, BadRequest<string>>> AddTestBlock(
        [FromServices] BlockchainService chain,
        [FromBody] AddTestBlockRequest req,
        CancellationToken ct)
    {
        try
        {
            var signatures = req.Signatures
                .Select(s =>
                    new BlockchainService.BlockSignature(s.InstitutionId, Convert.FromBase64String(s.SignedHashBase64)))
                .ToList();

            var block = await chain.AddTestBlockAsync(req.BlockId, req.TestId, req.InstitutionId, req.MaxScore,
                req.Name, signatures, ct);
            return TypedResults.Ok(block);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    internal static async Task<Results<Ok<BlockEntity>, BadRequest<string>>> AddTestResultBlock(
        [FromServices] BlockchainService chain,
        [FromBody] AddTestResultBlockRequest req,
        CancellationToken ct)
    {
        try
        {
            var signatures = req.Signatures
                .Select(s =>
                    new BlockchainService.BlockSignature(s.InstitutionId, Convert.FromBase64String(s.SignedHashBase64)))
                .ToList();

            var block = await chain.AddTestResultBlockAsync(
                req.BlockId,
                req.TestResultId,
                req.TestId,
                req.StudentId,
                req.Score,
                req.Timestamp,
                signatures,
                ct);

            return TypedResults.Ok(block);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
    
    internal static async Task<Results<Ok<ProposeBlockResponse>, BadRequest<string>>> ProposeBlock(
        [FromServices] BlockchainService chain,
        [FromBody] ProposeBlockRequest req,
        CancellationToken ct)
    {
        try
        {
            var hash = Convert.FromBase64String(req.HashBase64);
            var signedHash = Convert.FromBase64String(req.SignedHashBase64);
            
            using var rsa = System.Security.Cryptography.RSA.Create();
            rsa.ImportFromPem(req.ProposerPublicKeyPem);
            var publicKeyBytes = rsa.ExportRSAPublicKey();

            var resultSignedHash = chain.ProposeBlockAsync(req.Block, hash, signedHash, publicKeyBytes);
            
            return TypedResults.Ok(new ProposeBlockResponse(Convert.ToBase64String(resultSignedHash)));
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}