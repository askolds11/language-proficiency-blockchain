using System.Text.Json;

namespace language_proficiency_blockchain.HashModels.v1;

internal sealed record HashableTestResultV1(
    byte[] PrevHash,
    Guid TestId,
    Guid TestResultId,
    JsonDocument Score
) : HashableBlockBaseV1(PrevHash);