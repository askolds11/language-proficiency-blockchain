namespace language_proficiency_blockchain.endpoints;

/// <summary>
/// Endpoints
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Register the endpoints
    /// </summary>
    /// <param name="builder">IEndpointRouteBuilder</param>
    static abstract void MapEndpoint(IEndpointRouteBuilder builder);
}