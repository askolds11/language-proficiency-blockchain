namespace language_proficiency_blockchain.responses.Authentication;

/// <summary>
/// Response model for successful authentication.
/// </summary>
public class AuthenticationResponse
{
    /// <summary>
    /// JWT token for authenticated requests.
    /// </summary>
    public required string Token { get; init; }

    /// <summary>
    /// User identifier.
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// User email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Token expiration time in UTC.
    /// </summary>
    public required DateTime ExpiresAt { get; init; }
}
