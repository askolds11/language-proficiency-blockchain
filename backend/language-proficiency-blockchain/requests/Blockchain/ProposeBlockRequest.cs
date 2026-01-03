// language-proficiency-blockchain/requests/Blockchain/ProposeBlockRequest.cs
using JetBrains.Annotations;
using language_proficiency_blockchain.HashModels.Interfaces;

namespace language_proficiency_blockchain.requests.Blockchain;

[PublicAPI]
public sealed record ProposeBlockRequest(
    BlockBase Block,
    string HashBase64,
    string SignedHashBase64,
    string ProposerPublicKeyPem
);