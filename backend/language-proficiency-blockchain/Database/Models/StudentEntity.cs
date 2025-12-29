using System.ComponentModel.DataAnnotations;

namespace language_proficiency_blockchain.Database.Models;

/// <summary>
/// Represents a student/subject whose language proficiency test results are recorded.
/// </summary>
internal class StudentEntity
{
    /// <summary>
    /// External identifier of the student as referenced by <see cref="TestResultEntity"/>.
    /// </summary>
    [Key]
    public required Guid Id { get; init; }

    /// <summary>
    /// Optional human-readable name of the student.
    /// </summary>
    [MaxLength(256)]
    public required string? Name { get; init; }
    
    /// <summary>
    /// Optional human-readable surname of the student.
    /// </summary>
    [MaxLength(256)]
    public required string? Surname { get; init; }
}
