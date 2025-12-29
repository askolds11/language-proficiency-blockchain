using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace language_proficiency_blockchain.Database.Models;

/// <summary>
/// Represents an institution that participates in the blockchain.
/// </summary>
[Index(nameof(BlockId), IsUnique = true)]
internal class InstitutionEntity
{
    /// <summary>
    /// Unique identifier of the institution.
    /// </summary>
    [Key]
    public required Guid Id { get; init; }
    
    /// <summary>
    /// Foreign key to the block
    /// </summary>
    public required Guid? BlockId { get; init; }

    /// <summary>
    /// Public key of the institution in PEM format used to verify signatures.
    /// </summary>
    public required byte[] PublicKeyPem { get; init; }

    /// <summary>
    /// Optional human-readable address or endpoint of the institution.
    /// </summary>
    [MaxLength(512)]
    public required string Address { get; init; }
    
    /// <summary>
    /// Navigation to the related institution entity.
    /// </summary>
    [ForeignKey(nameof(BlockId))]
    public BlockEntity? Block { get; init; } = null!;
}
