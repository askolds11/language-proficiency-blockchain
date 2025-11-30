using System.ComponentModel.DataAnnotations;

namespace language_proficiency_blockchain.models;

/// <summary>
/// Represents a language proficiency test institution
/// </summary>
public class Institution
{
    /// <summary>
    /// Identifier of the institution
    /// </summary>
    [Key]
    [MaxLength(256)]
    public Guid Id { get; set; }

    /// <summary>
    /// Optional human-readable name for the institution.
    /// </summary>
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when this record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
