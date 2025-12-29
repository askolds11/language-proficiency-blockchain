using JetBrains.Annotations;

namespace language_proficiency_blockchain.requests.Blockchain;

[PublicAPI]
public sealed record AddInstitutionBlockRequest(
    Guid BlockId,
    Guid InstitutionId,
    string InstitutionName,
    IReadOnlyCollection<SignedByInstitution> Signatures
);