using JetBrains.Annotations;

namespace language_proficiency_blockchain.requests.Internal;

/// <summary>
/// Request for proposing a new test block.
/// </summary>
/// <param name="BlockId">Id of the new block</param>
/// <param name="TestId">Id of the test</param>
/// <param name="InstitutionId">Id of the institution</param>
/// <param name="MaxScore">Maximum score for the test</param>
/// <param name="Name">Name of the test</param>
[PublicAPI]
public sealed record AddTestRequest(
    Guid BlockId,
    Guid TestId,
    Guid InstitutionId,
    string MaxScore,
    string? Name
);
