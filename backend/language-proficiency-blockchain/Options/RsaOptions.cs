using System.ComponentModel.DataAnnotations;

namespace language_proficiency_blockchain.Options;

internal sealed class RsaOptions
{
    public const string Options = "RsaOptions";
    
    [Required]
    public required string PrivateKeyPath { get; set; }
    [Required]
    public required string PublicKeyPath { get; set; }
}