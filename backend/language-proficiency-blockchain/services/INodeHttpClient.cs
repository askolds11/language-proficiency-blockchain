using language_proficiency_blockchain.HashModels.Interfaces;
using language_proficiency_blockchain.responses.Blockchain;

namespace language_proficiency_blockchain.services;

/// <summary>
/// Abstraction for HTTP communication with other blockchain nodes.
/// </summary>
public interface INodeHttpClient
{
    /// <summary>
    /// Proposes a block to a remote node and retrieves its signed hash.
    /// </summary>
    Task<ProposeBlockResponse?> ProposeBlockAsync(
        string nodeAddress,
        BlockBase block,
        byte[] hash,
        byte[] signedHash,
        string proposerPublicKeyPem,
        CancellationToken ct = default);

    /// <summary>
    /// Sends a finalized block to a remote node.
    /// </summary>
    Task<bool> SendFinalizedBlockAsync<TRequest>(
        string nodeAddress,
        string endpoint,
        TRequest request,
        CancellationToken ct = default);
}