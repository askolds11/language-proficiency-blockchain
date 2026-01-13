using System.Reflection;
using language_proficiency_blockchain.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace language_proficiency_blockchain.Database;

internal class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<BlockEntity> Blocks { get; init; }
    public DbSet<InstitutionEntity> Institutions { get; init; }
    public DbSet<StudentEntity> Students { get; init; }
    public DbSet<TestEntity> Tests { get; init; }
    public DbSet<TestResultEntity> TestResults { get; init; }
    public DbSet<UserEntity> Users { get; init; }
    public DbSet<UserRoleAssociation> UserRoles { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<Institution>().HasIndex(n => n.Address).IsUnique(false);
        // modelBuilder.Entity<Institution>().HasIndex(n => n.PublicKeyPem).IsUnique();
        // modelBuilder.Entity<NodeApproval>().HasIndex(a => new { NodeId = a.InstitutionId, a.ApproverNodeId }).IsUnique();
        // modelBuilder.Entity<Block>().HasIndex(b => b.Hash).IsUnique();
        // modelBuilder.Entity<Block>().HasIndex(b => b.Index).IsUnique();
        // modelBuilder.Entity<Block>()
        //     .Property(b => b.Type)
        //     .HasConversion<string>()
        //     .HasMaxLength(64);
        // modelBuilder.Entity<TestResult>().HasIndex(t => t.DataHash);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
