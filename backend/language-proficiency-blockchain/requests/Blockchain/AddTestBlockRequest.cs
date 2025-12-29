using JetBrains.Annotations;

namespace language_proficiency_blockchain.requests.Blockchain;

[PublicAPI]
public sealed record AddTestBlockRequest(
    Guid BlockId,
    Guid TestId,
    Guid InstitutionId,
    string MaxScore,
    string? Name,
    IReadOnlyCollection<SignedByInstitution> Signatures
);