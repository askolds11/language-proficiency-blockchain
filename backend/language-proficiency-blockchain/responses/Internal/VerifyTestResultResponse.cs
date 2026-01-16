using JetBrains.Annotations;

namespace language_proficiency_blockchain.responses.Internal;

/// <summary>
/// Response for verifying test result data.
/// </summary>
/// <param name="IsValid">Whether the provided data matches the stored hash</param>
/// <param name="StoredHash">The hash stored in the blockchain (hex-encoded)</param>
/// <param name="ComputedHash">The hash computed from provided data (hex-encoded)</param>
[PublicAPI]
public sealed record VerifyTestResultResponse(
    bool IsValid,
    string StoredHash,
    string ComputedHash
);
