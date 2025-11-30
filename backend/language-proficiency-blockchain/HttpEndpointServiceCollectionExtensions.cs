using language_proficiency_blockchain.endpoints;
using ServiceScan.SourceGenerator;

namespace language_proficiency_blockchain;

/// <summary>
/// Automatically register endpoints
/// </summary>
/// <see href="https://renatogolia.com/2025/08/07/auto-register-aspnet-core-minimal-api-endpoints">Source</see>
public static partial class HttpEndpointServiceCollectionExtensions
{
    /// <summary>
    /// Maps endpoints in the app
    /// </summary>
    /// <param name="builder">Endpoint Route Builder</param>
    /// <returns>Endpoint Route Builder</returns>
    [GenerateServiceRegistrations(
        AssignableTo = typeof(IEndpoint),
        CustomHandler = nameof(MapEndpoint)
    )]
    public static partial IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder);

    /// <summary>
    /// Invokes the static <c>MapEndpoint</c> method on the specified endpoint type
    /// to register its routes on the provided builder.
    /// </summary>
    /// <param name="builder">Endpoint route builder.</param>
    /// <typeparam name="T">Endpoint type implementing <see cref="IEndpoint"/>.</typeparam>
    private static void MapEndpoint<T>(IEndpointRouteBuilder builder) where T : IEndpoint
    {
        T.MapEndpoint(builder);
    }
}
