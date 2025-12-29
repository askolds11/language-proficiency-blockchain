using language_proficiency_blockchain.HashModels.Interfaces;
using language_proficiency_blockchain.HashModels.v1;

namespace language_proficiency_blockchain.HashModels;

internal static class Hasher
{
    public static byte[] HashBlock(BlockBase obj)
    {
        return obj.Version switch
        {
            1 => HasherV1.CalculateHash(obj),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}