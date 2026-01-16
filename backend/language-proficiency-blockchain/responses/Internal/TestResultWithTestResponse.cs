using System.Text.Json;
using JetBrains.Annotations;

namespace language_proficiency_blockchain.responses.Internal;

/// <summary>
/// Response for a single test result with test details.
/// </summary>
/// <param name="TestResultId">ID of the test result</param>
/// <param name="TestId">ID of the test</param>
/// <param name="TestName">Name of the test (if available)</param>
/// <param name="TestMaxScore">Maximum score for the test</param>
/// <param name="InstitutionId">ID of the institution</param>
/// <param name="Score">Score achieved</param>
/// <param name="Timestamp">When the result was recorded</param>
/// <param name="BlockHash">Hash of the block containing this result (hex-encoded)</param>
/// <param name="PrevBlockHash">Hash of the previous block (hex-encoded)</param>
[PublicAPI]
public sealed record TestResultWithTestResponse(
    Guid TestResultId,
    Guid TestId,
    string? TestName,
    string TestMaxScore,
    Guid InstitutionId,
    JsonDocument Score,
    DateTimeOffset Timestamp,
    string BlockHash,
    string PrevBlockHash
);

