namespace language_proficiency_blockchain.HashModels.v1;

internal sealed record HashableInstitutionV1(
    byte[] PrevHash,
    Guid InstitutionId,
    string Name
) : HashableBlockBaseV1(PrevHash);