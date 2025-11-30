using System.ComponentModel.DataAnnotations;

namespace language_proficiency_blockchain.models;

/// <summary>
/// Represents a network node that participates in the blockchain (validators/proposers).
/// </summary>
public class Node
{
    /// <summary>
    /// Unique identifier of the node.
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Public key of the node in PEM format used to verify signatures.
    /// </summary>
    [Required]
    [MaxLength(2048)]
    public string PublicKeyPem { get; set; } = string.Empty;

    /// <summary>
    /// Optional human-readable address or endpoint of the node.
    /// </summary>
    [MaxLength(512)]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the node has been approved by the network.
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// UTC timestamp when the node entry was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
