using language_proficiency_blockchain.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace language_proficiency_blockchain.Database.Configurations;

/// <summary>
/// Entity configuration for UserEntity.
/// </summary>
internal class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
