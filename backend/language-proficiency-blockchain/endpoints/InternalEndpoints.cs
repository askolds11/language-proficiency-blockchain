using JetBrains.Annotations;
using language_proficiency_blockchain.Database;
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

        group.MapGet("ping", Ping)
            .AllowAnonymous();

        group.MapPost("assign-role", AssignRole)
            .AllowAnonymous();
        group.MapPost("student", AddStudent)
            .RequireAuthorization(language_proficiency_blockchain.Authorization.AuthorizationPolicies.OperatorOnly);

        group.MapGet("students", GetStudents)
            .RequireAuthorization(language_proficiency_blockchain.Authorization.AuthorizationPolicies.Everyone);

        group.MapPost("ping", Ping)
            .RequireAuthorization(language_proficiency_blockchain.Authorization.AuthorizationPolicies.Everyone);
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

    /// <summary>
    /// Get all students.
    /// </summary>
    /// <param name="dbContext">Database context.</param>
    /// <returns>
    /// 200 OK with list of all students.
    /// </returns>
    internal static async Task<Ok<IEnumerable<StudentEntity>>> GetStudents(
        [FromServices] AppDbContext dbContext)
    {
        var students = await dbContext.Students.ToListAsync();
        return TypedResults.Ok((IEnumerable<StudentEntity>)students);
    }

        return TypedResults.Ok();
    }

    internal static async Task<Results<Ok, BadRequest>> Ping()
    {
        return TypedResults.Ok();
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
}