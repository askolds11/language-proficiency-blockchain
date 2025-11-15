using language_proficiency_blockchain.responses;

namespace language_proficiency_blockchain.services;

/// <summary>
/// Example service that examples
/// </summary>
public class ExampleService
{
    /// <summary>
    /// Get the example
    /// </summary>
    /// <param name="id">Id of the example</param>
    /// <returns>Example if exists</returns>
    public async Task<ExampleResponse?> GetExample(int id)
    {
        // Do work
        await Task.Delay(500);
        return id < 10 ? new ExampleResponse(id, id.ToString()) : null;
    }
}