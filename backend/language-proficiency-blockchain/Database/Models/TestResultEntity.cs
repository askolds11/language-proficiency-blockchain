using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace language_proficiency_blockchain.Database.Models;

/// <summary>
/// Represents a single language proficiency test result that is stored on-chain.
/// </summary>
[Index(nameof(BlockId), IsUnique = true)]
internal class TestResultEntity
{
    /// <summary>
    /// Primary key of the record. Generated as a new <see cref="Guid"/> when the entity is created.
    /// </summary>
    [Key]
    public required Guid Id { get; init; }
    
    /// <summary>
    /// Foreign key to the test definition taken by the student.
    /// </summary>
    public required Guid TestId { get; init; }
    
    /// <summary>
    /// Foreign key to the block
    /// </summary>
    public required Guid BlockId { get; init; }

    /// <summary>
    /// Foreign key to the student who took the test.
    /// </summary>
    public required Guid StudentId { get; init; }

    /// <summary>
    /// The score or grade awarded for the test. Kept as a string to support formats such as "85", "C1", "7.5", etc.
    /// Optional; may be null if the score is not disclosed.
    /// </summary>
    [MaxLength(128)]
    public required string Score { get; init; }

    /// <summary>
    /// Timestamp of when the result was recorded.
    /// </summary>
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// Navigation to the related student entity.
    /// </summary>
    [ForeignKey(nameof(StudentId))]
    public required StudentEntity? Student { get; init; }
    
    /// <summary>
    /// Navigation to the related test entity.
    /// </summary>
    [ForeignKey(nameof(TestId))]
    public TestEntity TestEntity { get; init; } = null!;
}
