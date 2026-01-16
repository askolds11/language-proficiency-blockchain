using JetBrains.Annotations;

namespace language_proficiency_blockchain.requests.Internal;

/// <summary>
/// Request for proposing a new institution block.
/// </summary>
/// <param name="BlockId">Id of the new block</param>
/// <param name="InstitutionId">Id of the institution</param>
/// <param name="InstitutionName">Name of the institution</param>
[PublicAPI]
public sealed record AddInstitutionBlockRequest(
    Guid BlockId,
    Guid InstitutionId,
    string InstitutionName
);

