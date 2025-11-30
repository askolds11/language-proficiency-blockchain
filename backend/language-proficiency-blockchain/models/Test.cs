using System.ComponentModel.DataAnnotations;

namespace language_proficiency_blockchain.models;

/// <summary>
/// Represents a language proficiency test/exam definition or session.
/// </summary>
public class Test
{
    /// <summary>
    /// Identifier of the test (e.g., exam/session code) as referenced by TestResult.TestId.
    /// </summary>
    [Key]
    [MaxLength(256)]
    public Guid Id { get; set; }

    /// <summary>
    /// Optional human-readable name for the test.
    /// </summary>
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when this record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
