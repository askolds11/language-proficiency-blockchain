using language_proficiency_blockchain.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace language_proficiency_blockchain.Database.Configurations;

/// <summary>
/// Entity configuration for UserRoleAssociation.
/// </summary>
internal class UserRoleAssociationConfiguration : IEntityTypeConfiguration<UserRoleAssociation>
{
    public void Configure(EntityTypeBuilder<UserRoleAssociation> builder)
    {
        builder.HasKey(ura => ura.Id);

        builder.HasOne(ura => ura.User)
            .WithMany()
            .HasForeignKey(ura => ura.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ura => new { ura.UserId, ura.Role }).IsUnique();
    }
}
