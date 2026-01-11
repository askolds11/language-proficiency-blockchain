using System.ComponentModel.DataAnnotations;

namespace language_proficiency_blockchain.Options;

/// <summary>
/// Configuration options for JWT authentication.
/// </summary>
public class AuthenticationOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string Authentication = "Authentication";

    /// <summary>
    /// JWT secret key used for signing tokens.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string JwtSecret { get; init; }

    /// <summary>
    /// JWT issuer claim.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string JwtIssuer { get; init; }

    /// <summary>
    /// JWT audience claim.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string JwtAudience { get; init; }

    /// <summary>
    /// Token expiration time in minutes.
    /// </summary>
    [Range(1, int.MaxValue)]
    public required int TokenExpirationMinutes { get; init; }
}
