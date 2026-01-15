using JetBrains.Annotations;
using language_proficiency_blockchain.Database.Models;
using language_proficiency_blockchain.requests.Internal;
using language_proficiency_blockchain.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace language_proficiency_blockchain.endpoints;

/// <summary>
/// HTTP endpoints for private operations
/// </summary>
[PublicAPI]
[Authorize]
public class InternalEndpoints : IEndpoint
{
    /// <summary>
    /// Registers all internal endpoints under the <c>/internal</c> route group.
    /// </summary>
    /// <param name="builder">Endpoint route builder to map routes on.</param>
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("internal").WithTags("Internal").RequireAuthorization();

        group.MapPost("institution", AddInstitution)
            .RequireAuthorization(language_proficiency_blockchain.Authorization.AuthorizationPolicies.OperatorOnly);

        group.MapPost("ping", Ping)
            .RequireAuthorization(language_proficiency_blockchain.Authorization.AuthorizationPolicies.Everyone);
            
        group.MapPost("test", AddTest)
            .RequireAuthorization(language_proficiency_blockchain.Authorization.AuthorizationPolicies.OperatorOnly);

        group.MapPost("test-result", AddTestResult)
            .RequireAuthorization(language_proficiency_blockchain.Authorization.AuthorizationPolicies.VerificatorOrOperator);

        group.MapGet("ping", Ping)
            .AllowAnonymous();

        group.MapPost("assign-role", AssignRole)
            .AllowAnonymous();
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

    internal static async Task<Results<Ok, BadRequest>> Ping()
    {
        return TypedResults.Ok();
    }

    /// <summary>
    /// Proposes a new test block to the blockchain.
    /// </summary>
    /// <param name="internalService">Internal service.</param>
    /// <param name="req">Test payload.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// 200 OK with the created block if successful.
    /// 400 BadRequest if operation fails.
    /// </returns>
    internal static async Task<Results<Ok<BlockEntity>, BadRequest<string>>> AddTest(
        [FromServices] InternalService internalService,
        [FromBody] AddTestRequest req,
        CancellationToken ct)
    {
        try
        {
            var block = await internalService.ProposeTestBlockAsync(
                req.BlockId,
                req.TestId,
                req.InstitutionId,
                req.MaxScore,
                req.Name,
                ct);

            return TypedResults.Ok(block);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Proposes a new test result block to the blockchain.
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// 200 OK with the created block if successful.
    /// 400 BadRequest if operation fails.
    /// </returns>
    internal static async Task<Results<Ok<BlockEntity>, BadRequest<string>>> AddTestResult(
        [FromServices] InternalService internalService,
        [FromBody] AddTestResultRequest req,
        CancellationToken ct)
    {
        try
        {
            var block = await internalService.ProposeTestResultBlockAsync(
                req.BlockId,
                req.TestResultId,
                req.TestId,
                req.StudentId,
                req.Score,
                req.Timestamp,
                ct);

            return TypedResults.Ok(block);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="internalService">Internal service.</param>
    /// <param name="req">Assign role payload.</param>
    /// <returns>
    /// 200 OK if assigned successfully.
    /// 400 BadRequest if user not found.
    /// </returns>
    internal static async Task<Results<Ok, BadRequest<string>>> AssignRole(
        [FromServices] InternalService internalService,
        [FromBody] AssignRoleRequest req)
    {
        try
        {
            await internalService.AssignRoleAsync(req.UserId, req.Role);
            return TypedResults.Ok();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}