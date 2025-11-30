using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace language_proficiency_blockchain.models;

/// <summary>
/// Represents a block in the blockchain containing immutable, signed data.
/// </summary>
public class Block
{
    /// <summary>
    /// Unique identifier of the block.
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Sequential index of the block in the chain. The genesis block has index 0.
    /// </summary>
    public long Index { get; set; }

    /// <summary>
    /// The semantic type of the block (e.g., <see cref="BlockType.TestResult"/>, <see cref="BlockType.NodeAdded"/>).
    /// </summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BlockType Type { get; set; } // e.g., TestResult, NodeAdded

    /// <summary>
    /// Reference identifier related to the payload. For example, a <see cref="TestResult"/> Id for result blocks
    /// or a <see cref="Node"/> Id for node events.
    /// </summary>
    public Guid RefId { get; set; }

    /// <summary>
    /// Hash of the underlying business data (hex-encoded string, up to 128 chars).
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string DataHash { get; set; } = string.Empty;

    /// <summary>
    /// Hash of the previous block (hex-encoded). Empty for the genesis block.
    /// </summary>
    [MaxLength(128)]
    public string PrevHash { get; set; } = string.Empty;

    /// <summary>
    /// Hash of the current block (hex-encoded) calculated from the block contents.
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string Hash { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the node that created and signed the block.
    /// </summary>
    public Guid CreatedByNodeId { get; set; }

    /// <summary>
    /// Base64-encoded signature of the block content signed by <see cref="CreatedByNodeId"/>'s private key.
    /// </summary>
    [MaxLength(4096)]
    public string SignatureBase64 { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when the block was created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
