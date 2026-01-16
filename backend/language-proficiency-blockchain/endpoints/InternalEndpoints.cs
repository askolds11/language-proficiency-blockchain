using JetBrains.Annotations;
using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.requests.Internal;
using language_proficiency_blockchain.responses.Internal;
using language_proficiency_blockchain.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            .RequireAuthorization(Authorization.AuthorizationPolicies.OperatorOnly);

        group.MapPost("institution-block", AddInstitutionBlock)
            .RequireAuthorization(Authorization.AuthorizationPolicies.OperatorOnly);

        group.MapPost("ping", Ping)
            .RequireAuthorization(Authorization.AuthorizationPolicies.Everyone);
            
        group.MapPost("test", AddTest)
            .RequireAuthorization(Authorization.AuthorizationPolicies.OperatorOnly);

        group.MapPost("test-result", AddTestResult)
            .RequireAuthorization(Authorization.AuthorizationPolicies.VerificatorOrOperator);

        group.MapGet("ping", Ping)
            .AllowAnonymous();

        group.MapPost("assign-role", AssignRole)
            .AllowAnonymous();
        group.MapPost("student", AddStudent)
            .RequireAuthorization(Authorization.AuthorizationPolicies.OperatorOnly);

        group.MapGet("students", GetStudents)
            .RequireAuthorization(Authorization.AuthorizationPolicies.Everyone);
        
        group.MapPost("test-results/verify", VerifyTestResult)
            .RequireAuthorization(Authorization.AuthorizationPolicies.Everyone);
        
        group.MapGet("test-results/my", GetMyTestResults)
            .RequireAuthorization(Authorization.AuthorizationPolicies.Everyone);
        
        group.MapPost("test-results/share", CreateShareCode)
            .RequireAuthorization(Authorization.AuthorizationPolicies.Everyone);
        
        group.MapGet("test-results/shared/{code}", GetSharedTestResult)
            .AllowAnonymous();
        
        group.MapDelete("test-results/share/{code}", RevokeShareCode)
            .RequireAuthorization(Authorization.AuthorizationPolicies.Everyone);
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

    /// <summary>
    /// Proposes a new institution block to the blockchain.
    /// </summary>
    /// <param name="internalService">Internal service.</param>
    /// <param name="req">Institution block payload.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// 200 OK with the created block if successful.
    /// 400 BadRequest if operation fails.
    /// </returns>
    internal static async Task<Results<Ok<BlockResponse>, BadRequest<string>>> AddInstitutionBlock(
        [FromServices] InternalService internalService,
        [FromBody] requests.Internal.AddInstitutionBlockRequest req,
        CancellationToken ct)
    {
        try
        {
            var block = await internalService.ProposeInstitutionBlockAsync(
                req.BlockId,
                req.InstitutionId,
                req.InstitutionName,
                ct);

            return TypedResults.Ok(BlockResponse.FromEntity(block));
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Add a new student.
    /// </summary>
    /// <param name="internalService">Internal service.</param>
    /// <param name="req">Student payload.</param>
    /// <returns>
    /// 200 OK if added successfully
    /// </returns>
    internal static async Task<Results<Ok, BadRequest<string>>> AddStudent(
        [FromServices] InternalService internalService,
        [FromBody] AddStudentRequest req)
    {
        await internalService.AddStudent(req.Id, req.Name, req.Surname);
        return TypedResults.Ok();
    }

    /// <summary>
    /// Get all students.
    /// </summary>
    /// <param name="dbContext">Database context.</param>
    /// <returns>
    /// 200 OK with list of all students.
    /// </returns>
    internal static async Task<Ok<IEnumerable<StudentResponse>>> GetStudents(
        [FromServices] AppDbContext dbContext)
    {
        var students = await dbContext.Students.ToListAsync();
        return TypedResults.Ok(students.Select(StudentResponse.FromEntity));
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
    internal static async Task<Results<Ok<BlockResponse>, BadRequest<string>>> AddTest(
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

            return TypedResults.Ok(BlockResponse.FromEntity(block));
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Proposes a new test result block to the blockchain.
    /// </summary>
    /// <param name="internalService">Internal service.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// 200 OK with the created block if successful.
    /// 400 BadRequest if operation fails.
    /// </returns>
    internal static async Task<Results<Ok<BlockResponse>, BadRequest<string>>> AddTestResult(
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

            return TypedResults.Ok(BlockResponse.FromEntity(block));
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
    
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

    /// <summary>
    /// Verifies that test result data matches the stored blockchain hash.
    /// </summary>
    /// <param name="internalService">Internal service.</param>
    /// <param name="req">Verification request payload.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// 200 OK with verification result, or 404 if test result not found.
    /// </returns>
    internal static async Task<Results<Ok<VerifyTestResultResponse>, NotFound>> VerifyTestResult(
        [FromServices] InternalService internalService,
        [FromBody] VerifyTestResultRequest req,
        CancellationToken ct)
    {
        var result = await internalService.VerifyTestResultAsync(
            req.TestResultId, req.TestId, req.Score, req.PrevHashHex, ct);

        return result != null
            ? TypedResults.Ok(result)
            : TypedResults.NotFound();
    }

    /// <summary>
    /// Gets all test results for the authenticated user.
    /// </summary>
    /// <param name="internalService">Internal service.</param>
    /// <param name="context">HTTP context for user info.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// 200 OK with list of test results, or empty list if user has no linked student.
    /// </returns>
    internal static async Task<Ok<IReadOnlyList<TestResultWithTestResponse>>> GetMyTestResults(
        [FromServices] InternalService internalService,
        HttpContext context,
        CancellationToken ct)
    {
        var userIdClaim = context.User.FindFirst("sub")?.Value 
                          ?? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return TypedResults.Ok<IReadOnlyList<TestResultWithTestResponse>>([]);
        }

        var results = await internalService.GetUserTestResultsAsync(userId, ct);
        return TypedResults.Ok(results);
    }

    /// <summary>
    /// Creates a shareable code for a test result.
    /// </summary>
    /// <param name="internalService">Internal service.</param>
    /// <param name="req">Share code creation request.</param>
    /// <param name="context">HTTP context for user info.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// 200 OK with the generated share code, or 400 if invalid request.
    /// </returns>
    internal static async Task<Results<Ok<ShareCodeResponse>, BadRequest<string>>> CreateShareCode(
        [FromServices] InternalService internalService,
        [FromBody] CreateShareCodeRequest req,
        HttpContext context,
        CancellationToken ct)
    {
        var userIdClaim = context.User.FindFirst("sub")?.Value 
                          ?? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return TypedResults.BadRequest("User ID not found in token");
        }

        try
        {
            var result = await internalService.CreateShareCodeAsync(req.TestResultId, userId, req.ExpiresAt, ct);
            return TypedResults.Ok(result);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves a shared test result by share code. This endpoint is publicly accessible.
    /// </summary>
    /// <param name="internalService">Internal service.</param>
    /// <param name="code">The share code.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// 200 OK with the shared test result, or 404 if code is invalid/expired.
    /// </returns>
    internal static async Task<Results<Ok<SharedTestResultResponse>, NotFound>> GetSharedTestResult(
        [FromServices] InternalService internalService,
        string code,
        CancellationToken ct)
    {
        var result = await internalService.GetSharedTestResultAsync(code, ct);

        return result != null
            ? TypedResults.Ok(result)
            : TypedResults.NotFound();
    }

    /// <summary>
    /// Revokes a share code so it can no longer be used.
    /// </summary>
    /// <param name="internalService">Internal service.</param>
    /// <param name="code">The share code to revoke.</param>
    /// <param name="context">HTTP context for user info.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// 200 OK if revoked successfully, 404 if code not found or not owned by user.
    /// </returns>
    internal static async Task<Results<Ok, NotFound>> RevokeShareCode(
        [FromServices] InternalService internalService,
        string code,
        HttpContext context,
        CancellationToken ct)
    {
        var userIdClaim = context.User.FindFirst("sub")?.Value 
                          ?? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return TypedResults.NotFound();
        }

        var revoked = await internalService.RevokeShareCodeAsync(code, userId, ct);

        return revoked
            ? TypedResults.Ok()
            : TypedResults.NotFound();
    }
}