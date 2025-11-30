using System.ComponentModel.DataAnnotations;

namespace language_proficiency_blockchain.models;

/// <summary>
/// Represents a single language proficiency test result that is stored on-chain.
/// </summary>
public class TestResult
{
    /// <summary>
    /// Primary key of the record. Generated as a new <see cref="Guid"/> when the entity is created.
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Foreign key to the test definition taken by the student.
    /// </summary>
    [Required]
    public Guid TestId { get; set; }

    /// <summary>
    /// Navigation to the related test entity.
    /// </summary>
    public Test? Test { get; set; }

    /// <summary>
    /// Foreign key to the student who took the test.
    /// </summary>
    [Required]
    public Guid StudentId { get; set; }

    /// <summary>
    /// Navigation to the related student entity.
    /// </summary>
    public Student? Student { get; set; }

    /// <summary>
    /// Foreign key to the institution that issued/verified the test result.
    /// </summary>
    [Required]
    public Guid InstitutionId { get; set; }

    /// <summary>
    /// Navigation to the related institution entity.
    /// </summary>
    public Institution? Institution { get; set; }

    /// <summary>
    /// The score or grade awarded for the test. Kept as a string to support formats such as "85", "C1", "7.5", etc.
    /// Optional; may be null if the score is not disclosed.
    /// </summary>
    [MaxLength(128)]
    public string? Score { get; set; }

    /// <summary>
    /// Timestamp of when the result was recorded, in UTC.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identifier of the network node that submitted this result to the blockchain.
    /// </summary>
    [Required]
    public Guid SubmittedByNodeId { get; set; }

    /// <summary>
    /// Navigation to the node that submitted the result.
    /// </summary>
    public Node? SubmittedByNode { get; set; }

    /// <summary>
    /// Hash of the canonicalized test result payload for integrity verification on-chain.
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string DataHash { get; set; } = string.Empty;
}
