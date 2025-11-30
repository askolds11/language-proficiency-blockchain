using language_proficiency_blockchain.models;
using Microsoft.EntityFrameworkCore;

namespace language_proficiency_blockchain.data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Node> Nodes => Set<Node>();
    public DbSet<NodeApproval> NodeApprovals => Set<NodeApproval>();
    public DbSet<TestResult> TestResults => Set<TestResult>();
    public DbSet<Block> Blocks => Set<Block>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Node>().HasIndex(n => n.Address).IsUnique(false);
        modelBuilder.Entity<Node>().HasIndex(n => n.PublicKeyPem).IsUnique();
        modelBuilder.Entity<NodeApproval>().HasIndex(a => new { a.NodeId, a.ApproverNodeId }).IsUnique();
        modelBuilder.Entity<Block>().HasIndex(b => b.Hash).IsUnique();
        modelBuilder.Entity<Block>().HasIndex(b => b.Index).IsUnique();
        modelBuilder.Entity<Block>()
            .Property(b => b.Type)
            .HasConversion<string>()
            .HasMaxLength(64);
        modelBuilder.Entity<TestResult>().HasIndex(t => t.DataHash);
    }
}
