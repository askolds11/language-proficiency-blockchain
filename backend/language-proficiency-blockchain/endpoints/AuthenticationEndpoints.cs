using JetBrains.Annotations;
using language_proficiency_blockchain.requests.Authentication;
using language_proficiency_blockchain.responses.Authentication;
using language_proficiency_blockchain.services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace language_proficiency_blockchain.endpoints;

/// <summary>
/// HTTP endpoints for authentication operations such as login and register.
/// </summary>
[PublicAPI]
public class AuthenticationEndpoints : IEndpoint
{
    /// <summary>
    /// Registers all authentication endpoints under the <c>/auth</c> route group.
    /// </summary>
    /// <param name="builder">Endpoint route builder to map routes on.</param>
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("auth").WithTags("Authentication").AllowAnonymous();

        group.MapPost("register", Register);
        group.MapPost("login", Login);
    }

    internal static async Task<Results<Ok<AuthenticationResponse>, BadRequest<string>>> Register(
        [FromServices] IAuthenticationService authService,
        [FromBody] RegisterRequest req,
        CancellationToken ct)
    {
        try
        {
            var user = await authService.RegisterAsync(req.Email, req.Password, ct);
            var (token, expiresAt) = authService.GenerateToken(user);

            var response = new AuthenticationResponse
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                ExpiresAt = expiresAt
            };

            return TypedResults.Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Registration failed: {ex.Message}");
        }
    }

    internal static async Task<Results<Ok<AuthenticationResponse>, BadRequest<string>>> Login(
        [FromServices] IAuthenticationService authService,
        [FromBody] LoginRequest req,
        CancellationToken ct)
    {
        try
        {
            var user = await authService.AuthenticateAsync(req.Email, req.Password, ct);
            var (token, expiresAt) = authService.GenerateToken(user);

            var response = new AuthenticationResponse
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                ExpiresAt = expiresAt
            };

            return TypedResults.Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Login failed: {ex.Message}");
        }
    }
}
