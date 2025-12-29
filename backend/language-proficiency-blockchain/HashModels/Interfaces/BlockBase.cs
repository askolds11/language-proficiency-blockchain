namespace language_proficiency_blockchain.HashModels.Interfaces;

public abstract record BlockBase(
    byte[] PrevHash,
    int Version
) : IHashable;