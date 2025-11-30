namespace language_proficiency_blockchain.models;

/// <summary>
/// Defines the semantic category of a blockchain <see cref="Block"/>.
/// </summary>
public enum BlockType
{
    /// <summary>
    /// The first block in the chain used to bootstrap the ledger.
    /// </summary>
    Genesis = 0,

    /// <summary>
    /// A block that records a submitted language test result.
    /// </summary>
    TestResult = 1,

    /// <summary>
    /// A block that records addition/approval of a network node.
    /// </summary>
    NodeAdded = 2
}
