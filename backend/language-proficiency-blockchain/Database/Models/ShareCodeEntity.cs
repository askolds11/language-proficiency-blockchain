using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace language_proficiency_blockchain.Database.Models;

/// <summary>
/// Represents a shareable code for accessing a test result.
/// </summary>
[Index(nameof(Code), IsUnique = true)]
internal class ShareCodeEntity
{
    /// <summary>
    /// Primary key of the share code.
    /// </summary>
    [Key]
    public required Guid Id { get; init; }

    /// <summary>
    /// The short, readable share code (e.g., "ABC12345").
    /// </summary>
    [MaxLength(16)]
    public required string Code { get; init; }

    /// <summary>
    /// Foreign key to the test result being shared.
    /// </summary>
    public required Guid TestResultId { get; init; }

    /// <summary>
    /// The user ID who created this share code.
    /// </summary>
    public required Guid CreatedByUserId { get; init; }

    /// <summary>
    /// When the share code was created.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// When the share code expires.
    /// </summary>
    public required DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// Whether the share code has been revoked.
    /// </summary>
    public required bool IsRevoked { get; set; }

    /// <summary>
    /// Navigation to the related test result entity.
    /// </summary>
    [ForeignKey(nameof(TestResultId))]
    public TestResultEntity? TestResult { get; init; }

    /// <summary>
    /// Navigation to the user who created the share code.
    /// </summary>
    [ForeignKey(nameof(CreatedByUserId))]
    public UserEntity? CreatedByUser { get; init; }
}

