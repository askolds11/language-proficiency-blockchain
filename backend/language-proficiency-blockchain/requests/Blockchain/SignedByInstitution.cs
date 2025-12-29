using JetBrains.Annotations;

namespace language_proficiency_blockchain.requests.Blockchain;

[PublicAPI]
public sealed record SignedByInstitution(
    Guid InstitutionId,
    string SignedHashBase64
);