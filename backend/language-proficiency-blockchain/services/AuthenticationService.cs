using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using language_proficiency_blockchain.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace language_proficiency_blockchain.services;

/// <summary>
/// Service interface for handling authentication operations.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Registers a new user with the provided email and password.
    /// </summary>
    /// <param name="email">User email address.</param>
    /// <param name="password">User password.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created user entity.</returns>
    /// <exception cref="InvalidOperationException">If user with email already exists.</exception>
    Task<UserEntity> RegisterAsync(string email, string password, CancellationToken ct);

    /// <summary>
    /// Authenticates a user with the provided email and password.
    /// </summary>
    /// <param name="email">User email address.</param>
    /// <param name="password">User password.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The authenticated user entity.</returns>
    /// <exception cref="InvalidOperationException">If user not found or password is invalid.</exception>
    Task<UserEntity> AuthenticateAsync(string email, string password, CancellationToken ct);

    /// <summary>
    /// Generates a JWT token for the provided user.
    /// </summary>
    /// <param name="user">User entity.</param>
    /// <param name="roles">User roles to include in the token.</param>
    /// <returns>JWT token string and expiration time.</returns>
    (string token, DateTime expiresAt) GenerateToken(UserEntity user, IReadOnlyCollection<UserRole> roles);

    /// <summary>
    /// Gets all roles assigned to a user.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of roles assigned to the user.</returns>
    Task<IReadOnlyCollection<UserRole>> GetUserRolesAsync(Guid userId, CancellationToken ct);
}

/// <summary>
/// Implementation of authentication service with JWT support.
/// </summary>
internal class AuthenticationService(
    IAuthenticationOptions options,
    Microsoft.AspNetCore.Identity.IPasswordHasher<UserEntity> passwordHasher,
    language_proficiency_blockchain.Database.AppDbContext dbContext) : IAuthenticationService
{
    private readonly IAuthenticationOptions _options = options;
    private readonly Microsoft.AspNetCore.Identity.IPasswordHasher<UserEntity> _passwordHasher = passwordHasher;
    private readonly language_proficiency_blockchain.Database.AppDbContext _dbContext = dbContext;

    public async Task<UserEntity> RegisterAsync(string email, string password, CancellationToken ct)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Email == email, ct))
        {
            throw new InvalidOperationException($"User with email '{email}' already exists.");
        }

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var hashedPassword = _passwordHasher.HashPassword(user, password);
        
        // Update the user with the hashed password by creating a new instance
        user = new UserEntity
        {
            Id = user.Id,
            Email = user.Email,
            PasswordHash = hashedPassword,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(ct);

        return user;
    }

    public async Task<UserEntity> AuthenticateAsync(string email, string password, CancellationToken ct)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user == null)
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        return user;
    }

    public (string token, DateTime expiresAt) GenerateToken(UserEntity user, IReadOnlyCollection<UserRole> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.JwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(_options.TokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // Add each role as a claim
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: _options.JwtIssuer,
            audience: _options.JwtAudience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, expiresAt);
    }

    public async Task<IReadOnlyCollection<UserRole>> GetUserRolesAsync(Guid userId, CancellationToken ct)
    {
        var roles = await _dbContext.UserRoles
            .Where(ura => ura.UserId == userId)
            .Select(ura => ura.Role)
            .ToListAsync(ct);

        return roles.AsReadOnly();
    }
}

/// <summary>
/// Interface for authentication options.
/// </summary>
internal interface IAuthenticationOptions
{
    string JwtSecret { get; }
    string JwtIssuer { get; }
    string JwtAudience { get; }
    int TokenExpirationMinutes { get; }
}

/// <summary>
/// Wrapper for authentication options from configuration.
/// </summary>
internal class AuthenticationOptionsWrapper(
    Microsoft.Extensions.Options.IOptions<language_proficiency_blockchain.Options.AuthenticationOptions> options) : IAuthenticationOptions
{
    private readonly language_proficiency_blockchain.Options.AuthenticationOptions _options = options.Value;

    public string JwtSecret => _options.JwtSecret;
    public string JwtIssuer => _options.JwtIssuer;
    public string JwtAudience => _options.JwtAudience;
    public int TokenExpirationMinutes => _options.TokenExpirationMinutes;
}
