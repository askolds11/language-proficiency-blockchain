using System.Text.Json;
using JetBrains.Annotations;

namespace language_proficiency_blockchain.responses.Internal;

/// <summary>
/// Response for retrieving a shared test result.
/// </summary>
/// <param name="TestResultId">ID of the test result</param>
/// <param name="TestId">ID of the test</param>
/// <param name="StudentId">ID of the student</param>
/// <param name="Score">Score achieved</param>
/// <param name="Timestamp">When the result was recorded</param>
/// <param name="TestName">Name of the test (if available)</param>
/// <param name="InstitutionId">ID of the institution</param>
/// <param name="BlockHash">Hash of the block containing this result (hex-encoded)</param>
[PublicAPI]
public sealed record SharedTestResultResponse(
    Guid TestResultId,
    Guid TestId,
    Guid StudentId,
    JsonDocument Score,
    DateTimeOffset Timestamp,
    string? TestName,
    Guid InstitutionId,
    string BlockHash,
    string PrevBlockHash
);

