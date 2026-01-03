using language_proficiency_blockchain.HashModels.Interfaces;

namespace language_proficiency_blockchain.services;

/// <summary>
/// Repository abstraction for node communication operations.
/// Enables parallel HTTP calls and testability.
/// </summary>
public interface INodeRepository
{
    /// <summary>
    /// Collects signatures from nodes in parallel until the threshold is met.
    /// </summary>
    /// <param name="block">The block to propose</param>
    /// <param name="hash">The hash of the block</param>
    /// <param name="signedHash">The signed hash by the proposer</param>
    /// <param name="proposerPublicKeyPem">The proposer's public key in PEM format</param>
    /// <param name="threshold">Number of signatures required</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Collection of signatures from nodes</returns>
    Task<IReadOnlyCollection<CollectedSignature>> CollectSignaturesAsync(
        BlockBase block,
        byte[] hash,
        byte[] signedHash,
        string proposerPublicKeyPem,
        int threshold,
        CancellationToken ct = default);

    /// <summary>
    /// Broadcasts a finalized block to all nodes in parallel.
    /// </summary>
    Task BroadcastFinalizedBlockAsync<TRequest>(
        string endpoint,
        TRequest request,
        CancellationToken ct = default);
}

/// <summary>
/// Represents a collected signature from a node.
/// </summary>
public sealed record CollectedSignature(Guid InstitutionId, byte[] SignedHash);