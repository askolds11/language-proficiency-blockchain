using JetBrains.Annotations;

namespace language_proficiency_blockchain.requests.Internal;

/// <summary>
/// Request for adding an institution.
/// </summary>
/// <param name="Id">Id of the institution</param>
/// <param name="Name">Name of the institution</param>
/// <param name="Address">Address of the institution</param>
/// <param name="PublicKeyPem">Public key of the institution</param>
[PublicAPI]
public sealed record AddInstitutionRequest(
    Guid Id,
    string Name,
    string Address,
    string PublicKeyPem
);