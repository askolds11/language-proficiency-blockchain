using language_proficiency_blockchain.HashModels.Interfaces;

namespace language_proficiency_blockchain.HashModels;

/// <summary>
/// Record class used for determining block's version
/// </summary>
/// <param name="PrevHash">Hash of the previous block</param>
/// <param name="Version">Version of the block</param>
internal sealed record VersionBlockBase(
    byte[] PrevHash,
    int Version
) : BlockBase(PrevHash, Version);