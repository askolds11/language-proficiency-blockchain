using System.Text.Json;
using JetBrains.Annotations;

namespace language_proficiency_blockchain.requests.Internal;

/// <summary>
/// Request for verifying test result data against stored hash.
/// </summary>
/// <param name="TestResultId">ID of the test result to verify</param>
/// <param name="TestId">ID of the test</param>
/// <param name="Score">Score achieved in the test</param>
/// <param name="PrevHashHex">Hash of the previous block (hex-encoded)</param>
[PublicAPI]
public sealed record VerifyTestResultRequest(
    Guid TestResultId,
    Guid TestId,
    JsonDocument Score,
    string PrevHashHex
);
