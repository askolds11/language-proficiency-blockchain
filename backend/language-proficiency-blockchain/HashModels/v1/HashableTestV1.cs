namespace language_proficiency_blockchain.HashModels.v1;

internal sealed record HashableTestV1(
    byte[] PrevHash,
    Guid InstitutionId,
    Guid TestId,
    string MaxScore
) : HashableBlockBaseV1(PrevHash);