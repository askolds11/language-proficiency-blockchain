using JetBrains.Annotations;
using language_proficiency_blockchain.requests.Internal;
using language_proficiency_blockchain.services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace language_proficiency_blockchain.endpoints;

/// <summary>
/// HTTP endpoints for private operations
/// </summary>
[PublicAPI]
public class InternalEndpoints : IEndpoint
{
    /// <summary>
    /// Registers all internal endpoints under the <c>/internal</c> route group.
    /// </summary>
    /// <param name="builder">Endpoint route builder to map routes on.</param>
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("internal").WithTags("Internal");

        group.MapPost("institution", AddInstitution);
        // group.MapPost("nodes/{id:guid}/approve", ApproveNode);
        // group.MapGet("nodes", ListNodes);
        // group.MapGet("chain", GetChain);
        // group.MapGet("node-info", GetLocalNodeInfo);

        // group.MapPost("results", SubmitResult);
        // group.MapGet("results/{id:guid}", GetResult);
    }

    /// <summary>
    /// Add a new institution.
    /// </summary>
    /// <param name="internalService">Internal service.</param>
    /// <param name="req">Institution payload.</param>
    /// <returns>
    /// 200 OK if added successfully
    /// </returns>
    internal static async Task<Results<Ok, BadRequest<string>>> AddInstitution(
        [FromServices] InternalService internalService,
        [FromBody] AddInstitutionRequest req)
    {
        await internalService.AddInstitution(req.Id, req.Name, req.Address, req.PublicKeyPem);

        return TypedResults.Ok();
    }
}