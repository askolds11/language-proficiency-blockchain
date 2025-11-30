using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace language_proficiency_blockchain.models;

/// <summary>
/// Represents an approval action where an existing node approves a candidate node.
/// </summary>
public class NodeApproval
{
    /// <summary>
    /// Unique identifier of the node-approval record.
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the node being approved.
    /// </summary>
    [Required]
    public Guid NodeId { get; set; }

    /// <summary>
    /// Identifier of the node that performs the approval.
    /// </summary>
    [Required]
    public Guid ApproverNodeId { get; set; }

    /// <summary>
    /// Base64-encoded signature proving the approver's intent.
    /// </summary>
    [Required]
    [MaxLength(4096)]
    public string SignatureBase64 { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when the approval was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation to the node being approved.
    /// </summary>
    [ForeignKey(nameof(NodeId))]
    public Node? Node { get; set; }

    /// <summary>
    /// Navigation to the node that approved the candidate.
    /// </summary>
    [ForeignKey(nameof(ApproverNodeId))]
    public Node? ApproverNode { get; set; }
}
