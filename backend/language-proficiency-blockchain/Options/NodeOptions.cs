namespace language_proficiency_blockchain.Options;

/// <summary>
/// Configuration options for the local node.
/// </summary>
public class NodeOptions
{
    /// <summary>
    /// The address of this node (used to identify self in the network).
    /// </summary>
    public required string Address { get; init; }
    
    /// <summary>
    /// The institution ID of this node (if it's an institution node).
    /// </summary>
    public Guid? InstitutionId { get; init; }
}

