using JetBrains.Annotations;

namespace language_proficiency_blockchain.responses.Blockchain;

[PublicAPI]
public sealed record ProposeBlockResponse(
    string SignedHashBase64
);