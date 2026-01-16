using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace language_proficiency_blockchain.Database.Models;

/// <summary>
/// Represents a user in the system with authentication credentials.
/// </summary>
[Index(nameof(StudentId), IsUnique = true)]
public class UserEntity
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    [Key]
    public required Guid Id { get; init; }

    /// <summary>
    /// Optional link to a student record. Null if the user is not a student.
    /// </summary>
    public Guid? StudentId { get; init; }

    /// <summary>
    /// Email address of the user (unique).
    /// </summary>
    [MaxLength(256)]
    public required string Email { get; init; }

    /// <summary>
    /// Hashed password of the user.
    /// </summary>
    [MaxLength(256)]
    public required string PasswordHash { get; init; }

    /// <summary>
    /// Creation timestamp of the user.
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last update timestamp of the user.
    /// </summary>
    public required DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Navigation to the linked student record (if any).
    /// </summary>
    [ForeignKey(nameof(StudentId))]
    internal StudentEntity? Student { get; init; }
}
