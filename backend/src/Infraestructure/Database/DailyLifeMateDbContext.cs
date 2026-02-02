using DailyLifeMate.Domain.Core;
using Microsoft.EntityFrameworkCore;

namespace DailyLifeMate.Infrastructure.Persistence;


public class DailyLifeMateDbContext : DbContext
{
    // Configuration 
    public DailyLifeMateDbContext(DbContextOptions<DailyLifeMateDbContext> options) 
        : base(options) { }

    // DB Tables
    public DbSet<Context> Contexts => Set<Context>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemMetadata> ItemMetadatas => Set<ItemMetadata>();

    // Defines the DB rules
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Configure Context (The Category)
        modelBuilder.Entity<Context>(context =>
        {
            context.HasKey(e => e.Id);
            // Convert Enum to a string in the DB
            context.Property(e => e.Type)
                  .HasConversion<string>()
                  .HasMaxLength(50);
            
            context.HasIndex(e => e.Type).IsUnique();
        });

        // 2. Configure Item (The Feature)
        modelBuilder.Entity<Item>(item =>
        {
            item.HasKey(e => e.Id);
            item.Property(e => e.Title).IsRequired().HasMaxLength(255);
            
            // Relationship: Item belongs to one Context
            item.HasOne(i => i.Context)
                  .WithMany(c => c.Items)
                  .HasForeignKey(i => i.ContextId);
        });

        // 3. Configure Metadata
        modelBuilder.Entity<ItemMetadata>(metadata =>
        {
            metadata.HasKey(e => e.Id);
            
            // Composite Index for speed: makes looking up keys for an item very fast
            metadata.HasIndex(e => new { e.ItemId, e.Key }).IsUnique();

            metadata.Property(e => e.Key).HasMaxLength(100);
            metadata.Property(e => e.Value).HasMaxLength(1000);

            // Relationship: Metadata belongs to one Item
            metadata.HasOne(m => m.Item)
                  .WithMany(i => i.Metadata)
                  .HasForeignKey(m => m.ItemId)
                  .OnDelete(DeleteBehavior.Cascade); // Delete item = delete metadata
        });
    }
}