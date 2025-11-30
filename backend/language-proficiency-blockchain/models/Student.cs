using System.ComponentModel.DataAnnotations;

namespace language_proficiency_blockchain.models;

/// <summary>
/// Represents a student/subject whose language proficiency test results are recorded.
/// </summary>
public class Student
{
    /// <summary>
    /// External identifier of the student as referenced by <see cref="TestResult"/>.
    /// Kept as string to match existing data model.
    /// </summary>
    [Key]
    [MaxLength(256)]
    public Guid Id { get; set; }

    /// <summary>
    /// Optional human-readable name of the student.
    /// </summary>
    [MaxLength(256)]
    public string? Name { get; set; }

    /// <summary>
    /// Optional additional metadata (email, alias, etc.).
    /// </summary>
    [MaxLength(512)]
    public string? Metadata { get; set; }

    /// <summary>
    /// UTC timestamp when the record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
