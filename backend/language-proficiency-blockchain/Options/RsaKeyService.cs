using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace language_proficiency_blockchain.Options;

internal class RsaKeyMonitor : IOptionsMonitor<RsaKeyHolder>
{
    public RsaKeyMonitor(IOptionsMonitor<RsaOptions> source)
    {
        CurrentValue = Convert(source.CurrentValue);
        source.OnChange(opts =>
        {
            CurrentValue = Convert(opts);
            OnChangeCallbacks?.Invoke(CurrentValue, string.Empty);
        });
    }

    private static RsaKeyHolder Convert(RsaOptions opts)
    {
        var holder = new RsaKeyHolder
        {
            PublicKey = RSA.Create(),
            PrivateKey = RSA.Create()
        };

        holder.PublicKey.ImportFromPem(File.ReadAllText(opts.PublicKeyPath));
        holder.PrivateKey.ImportFromPem(File.ReadAllText(opts.PrivateKeyPath));

        return holder;
    }

    private event Action<RsaKeyHolder, string>? OnChangeCallbacks;

    public RsaKeyHolder CurrentValue { get; private set; }

    public RsaKeyHolder Get(string? name) => CurrentValue;

    public IDisposable OnChange(Action<RsaKeyHolder, string> listener)
    {
        OnChangeCallbacks += listener;
        return new Disposable(() => OnChangeCallbacks -= listener);
    }

    private class Disposable(Action dispose) : IDisposable
    {
        public void Dispose() => dispose();
    }
}