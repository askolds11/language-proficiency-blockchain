using System.Text.Json;
using JetBrains.Annotations;

namespace language_proficiency_blockchain.requests.Blockchain;

[PublicAPI]
public sealed record AddTestResultBlockRequest(
    Guid BlockId,
    Guid TestResultId,
    Guid TestId,
    Guid StudentId,
    JsonDocument Score,
    DateTimeOffset Timestamp,
    IReadOnlyCollection<SignedByInstitution> Signatures
);