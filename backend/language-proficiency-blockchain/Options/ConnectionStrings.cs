namespace language_proficiency_blockchain.Options;

internal class ConnectionStringsOptions
{
    public const string ConnectionStrings = "ConnectionStrings";

    public required string AppDb { get; init; }
}