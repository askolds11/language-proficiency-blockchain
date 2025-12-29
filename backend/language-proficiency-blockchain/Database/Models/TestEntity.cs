using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace language_proficiency_blockchain.Database.Models;

/// <summary>
/// Represents a language proficiency test/exam definition or session.
/// </summary>
[Index(nameof(BlockId), IsUnique = true)]
internal class TestEntity
{
    /// <summary>
    /// Identifier of the test (e.g., exam/session code) as referenced by TestResult.TestId.
    /// </summary>
    [Key]
    public required Guid Id { get; init; }
    
    /// <summary>
    /// Foreign key to the institution
    /// </summary>
    public required Guid InstitutionId { get; init; }
    
    /// <summary>
    /// Foreign key to the block
    /// </summary>
    public required Guid BlockId { get; init; }
    
    /// <summary>
    /// Optional human-readable name for the test.
    /// </summary>
    [MaxLength(256)]
    public required string? Name { get; init; }
    
    /// <summary>
    /// Max score for the test
    /// </summary>
    [MaxLength(128)]
    public required string MaxScore { get; init; }
    
    /// <summary>
    /// Navigation to the related institution entity.
    /// </summary>
    [ForeignKey(nameof(InstitutionId))]
    public InstitutionEntity Institution { get; init; } = null!;
    
    /// <summary>
    /// Navigation to the related block entity.
    /// </summary>
    [ForeignKey(nameof(BlockId))]
    public BlockEntity Block { get; init; } = null!;
}
