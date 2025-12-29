using System.Security.Cryptography;
using language_proficiency_blockchain.Options;

namespace language_proficiency_blockchain.Tests;

internal class RsaKeyFixture: IDisposable
{
    private const string PrivateRsaKey =
        """
        -----BEGIN PRIVATE KEY-----
        MIIBVAIBADANBgkqhkiG9w0BAQEFAASCAT4wggE6AgEAAkEAsC4LoXCyK77RuqBj
        Nr/UUckyi5wm4pivBvVwHzuB18LXEH5HEzUouWNzz16oeeUjptm+EJYUgs3v3zhT
        VMWmCwIDAQABAkEAicqpjA1xPGeU7ursTfDApWq/3pM1knoqQj4KAFNxXQaZEw4L
        MXzgbdqN3us0Gp55moayqRQNF5/NEW06UTMpgQIhANmlMGgRQ1GUFHpqp1Bbftjd
        nzjQB5/Ykr9jz9ZuhOzLAiEAzzoxeft/G8po5tYzEP0jgqdigATk8tYMRzSj9T0C
        Q8ECICvi6lrhuEBX6rUwkmJawL48GiIbmJ37zsN2/e7QRE93AiAt3vjSwqwJT83W
        wzV8njw9EKZKJkszwdPn8ywT/hRBQQIgVdShsfAt3VqppXnOhbSCIRi/PF6Yb8EZ
        nKvoWbIdPjE=
        -----END PRIVATE KEY-----
        """;
    
    private const string PublicRsaKey =
        """
        -----BEGIN PUBLIC KEY-----
        MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBALAuC6Fwsiu+0bqgYza/1FHJMoucJuKY
        rwb1cB87gdfC1xB+RxM1KLljc89eqHnlI6bZvhCWFILN7984U1TFpgsCAwEAAQ==
        -----END PUBLIC KEY-----
        """;

    public RSA PrivateKey { get; }
    public RSA PublicKey { get; }
    public TestOptionsMonitor<RsaKeyHolder> RsaOptionsMonitor { get; }
    
    public RsaKeyFixture()
    {
        PrivateKey = RSA.Create();
        PrivateKey.ImportFromPem(PrivateRsaKey);
        PublicKey = RSA.Create();
        PublicKey.ImportFromPem(PublicRsaKey);
        var rsaKeyHolder = new RsaKeyHolder
        {
            PrivateKey = PrivateKey,
            PublicKey = PublicKey
        };
        RsaOptionsMonitor = new TestOptionsMonitor<RsaKeyHolder>(rsaKeyHolder);
    }

    public void Dispose()
    {
        PrivateKey.Dispose();
        PublicKey.Dispose();
    }
}