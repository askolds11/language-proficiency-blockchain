using TUnit.Core.Interfaces;

namespace language_proficiency_blockchain.Tests;

public class TestContainerLimit : IParallelLimit
{
    public int Limit => 1;
}