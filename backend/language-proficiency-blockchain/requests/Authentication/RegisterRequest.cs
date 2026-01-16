using System.ComponentModel.DataAnnotations;

namespace language_proficiency_blockchain.requests.Authentication;

/// <summary>
/// Request model for user registration.
/// </summary>
public class RegisterRequest
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
    [MinLength(6)]
    public required string Password { get; init; }

    /// <summary>
    /// Optional student ID to link this user to an existing student record.
    /// </summary>
    public Guid? StudentId { get; init; }
}
