using System.Text.Json;
using JetBrains.Annotations;

namespace language_proficiency_blockchain.requests.Internal;

/// <summary>
/// Request for proposing a new test result block.
/// </summary>
/// <param name="BlockId">Id of the new block</param>
/// <param name="TestResultId">Id of the test result</param>
/// <param name="TestId">Id of the test</param>
/// <param name="StudentId">Id of the student</param>
/// <param name="Score">Score as a JSON document</param>
/// <param name="Timestamp">Timestamp of the test result</param>
[PublicAPI]
public sealed record AddTestResultRequest(
    Guid BlockId,
    Guid TestResultId,
    Guid TestId,
    Guid StudentId,
    JsonDocument Score,
    DateTimeOffset Timestamp
);
