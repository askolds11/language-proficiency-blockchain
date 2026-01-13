using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace language_proficiency_blockchain.Database.Models;

/// <summary>
/// Represents a many-to-many relationship between a user and a role.
/// </summary>
public class UserRoleAssociation
{
    /// <summary>
    /// Unique identifier for the association.
    /// </summary>
    [Key]
    public required Guid Id { get; init; }

    /// <summary>
    /// Foreign key to the user.
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Navigation property to the user.
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public UserEntity? User { get; init; }

    /// <summary>
    /// The role assigned to the user.
    /// </summary>
    public required UserRole Role { get; init; }

    /// <summary>
    /// Timestamp when the role was assigned.
    /// </summary>
    public required DateTime AssignedAt { get; init; }
}
