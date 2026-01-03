using language_proficiency_blockchain.HashModels.Interfaces;
using language_proficiency_blockchain.requests.Blockchain;
using language_proficiency_blockchain.responses.Blockchain;

namespace language_proficiency_blockchain.services;

/// <summary>
/// HTTP client implementation for communicating with other blockchain nodes.
/// </summary>
internal sealed class NodeHttpClient(IHttpClientFactory httpClientFactory) : INodeHttpClient
{
    public async Task<ProposeBlockResponse?> ProposeBlockAsync(
        string nodeAddress,
        BlockBase block,
        byte[] hash,
        byte[] signedHash,
        string proposerPublicKeyPem,
        CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(nodeAddress);

        var request = new ProposeBlockRequest(
            block,
            Convert.ToBase64String(hash),
            Convert.ToBase64String(signedHash),
            proposerPublicKeyPem);

        try
        {
            var response = await client.PostAsJsonAsync("blockchain/blocks/propose", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ProposeBlockResponse>(ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> SendFinalizedBlockAsync<TRequest>(
        string nodeAddress,
        string endpoint,
        TRequest request,
        CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(nodeAddress);

        try
        {
            var response = await client.PostAsJsonAsync(endpoint, request, ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}