using JetBrains.Annotations;

namespace language_proficiency_blockchain.requests.Internal;

/// <summary>
/// Request for adding a student.
/// </summary>
/// <param name="Id">Id of the student</param>
/// <param name="Name">Name of the student</param>
/// <param name="Surname">Surname of the student</param>
[PublicAPI]
public sealed record AddStudentRequest(
    Guid Id,
    string? Name,
    string? Surname
);
