using System.ComponentModel.DataAnnotations;

namespace language_proficiency_blockchain.requests.Authentication;

/// <summary>
/// Request model for user login.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Email address of the user.
    /// </summary>
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    /// Password for the user account.
    /// </summary>
    [Required]
    public required string Password { get; init; }
}
