namespace language_proficiency_blockchain.responses;

/// <summary>
/// Example response
/// </summary>
/// <param name="Id">Id of the Example</param>
/// <param name="Name">Name of the Example</param>
/// <example>{"id": 1, "name": "1"}</example>
public record ExampleResponse(int Id, string Name);