using JetBrains.Annotations;

namespace language_proficiency_blockchain.requests.Internal;

/// <summary>
/// Request for creating a share code for a test result.
/// </summary>
/// <param name="TestResultId">ID of the test result to share</param>
/// <param name="ExpiresAt">When the share code should expire</param>
[PublicAPI]
public sealed record CreateShareCodeRequest(
    Guid TestResultId,
    DateTimeOffset ExpiresAt
);

