using Microsoft.Extensions.Options;

namespace language_proficiency_blockchain.Tests;

internal class TestOptionsMonitor<T>(T currentValue) : IOptionsMonitor<T>
    where T : class
{
    public T Get(string? name)
    {
        return CurrentValue;
    }

    public IDisposable OnChange(Action<T, string> listener)
    {
        throw new NotImplementedException();
    }

    public T CurrentValue { get; } = currentValue;
}