using JetBrains.Annotations;

namespace language_proficiency_blockchain.responses.Internal;

/// <summary>
/// Response for creating a share code.
/// </summary>
/// <param name="Code">The generated share code</param>
/// <param name="ExpiresAt">When the share code expires</param>
[PublicAPI]
public sealed record ShareCodeResponse(
    string Code,
    DateTimeOffset ExpiresAt
);

