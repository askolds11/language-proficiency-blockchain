namespace language_proficiency_blockchain.Database.Models;

/// <summary>
/// Defines the semantic category of a blockchain <see cref="BlockEntity"/>.
/// </summary>
internal enum BlockType
{
    /// <summary>
    /// The first block in the chain used to bootstrap the ledger.
    /// </summary>
    FirstBlock,

    /// <summary>
    /// A block that records a test
    /// </summary>
    Institution,
    
    /// <summary>
    /// A block that records a test
    /// </summary>
    Test,
    
    /// <summary>
    /// A block that records a submitted language test result.
    /// </summary>
    TestResult,
}
