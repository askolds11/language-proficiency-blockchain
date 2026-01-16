using JetBrains.Annotations;
using language_proficiency_blockchain.Database.Models;

namespace language_proficiency_blockchain.responses.Internal;

/// <summary>
/// Response DTO for a student.
/// </summary>
/// <param name="Id">Unique identifier of the student</param>
/// <param name="Name">Optional name of the student</param>
/// <param name="Surname">Optional surname of the student</param>
[PublicAPI]
public sealed record StudentResponse(
    Guid Id,
    string? Name,
    string? Surname
)
{
    /// <summary>
    /// Creates a StudentResponse from a StudentEntity.
    /// </summary>
    internal static StudentResponse FromEntity(StudentEntity entity) => new(
        Id: entity.Id,
        Name: entity.Name,
        Surname: entity.Surname
    );
}

