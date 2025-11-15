using language_proficiency_blockchain.requests;
using language_proficiency_blockchain.responses;
using language_proficiency_blockchain.services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace language_proficiency_blockchain.endpoints;

/// <inheritdoc/>
public class ExampleEndpoint : IEndpoint
{
    /// <inheritdoc/>
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("example").WithTags("Example");
        group.MapGet("{id:int}", ExampleGet);
        group.MapPost("", ExamplePost);
    }

    /// <summary>
    /// Example endpoint that returns an example
    /// </summary>
    /// <param name="exampleService">ExampleService</param>
    /// <param name="id">Example's id</param>
    /// <returns>Ok, if id &lt; 10, otherwise not found</returns>
    /// <response code="200">Returns example if id &lt; 10</response>
    /// <response code="404">If id &gt;= 10</response>
    public static async Task<Results<Ok<ExampleResponse>, NotFound>> ExampleGet(
        [FromServices] ExampleService exampleService,
        [FromRoute] int id
    )
    {
        var example = await exampleService.GetExample(id);

        if (example == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(example);
    }
    
    /// <summary>
    /// Example endpoint that returns an example
    /// </summary>
    /// <param name="exampleService">ExampleService</param>
    /// <param name="request">Example</param>
    /// <returns>Ok, if id &lt; 10, otherwise not found</returns>
    /// <response code="200">Returns example if id &lt; 10</response>
    /// <response code="404">If id &gt;= 10</response>
    public static async Task<Results<Ok<ExampleResponse>, NotFound>> ExamplePost(
        [FromServices] ExampleService exampleService,
        [FromBody] ExampleRequest request
    )
    {
        var example = await exampleService.GetExample(request.Id);

        if (example == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(example);
    }
}