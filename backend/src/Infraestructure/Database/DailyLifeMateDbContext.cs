using DailyLifeMate.Domain.Core.Models;
using DailyLifeMate.Engine.Features.Series.Models;

using Microsoft.EntityFrameworkCore;

namespace DailyLifeMate.Infrastructure.Database;

public class DailyLifeMateDbContext : DbContext
{
    public DailyLifeMateDbContext(DbContextOptions<DailyLifeMateDbContext> options)
        : base(options) { }

    // Table Definitions
    public DbSet<DashboardItem> DashboardItems => Set<DashboardItem>();
    public DbSet<Anime> Animes => Set<Anime>();
    public DbSet<Context> Contexts => Set<Context>();

    // The Rules (Configuration)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Context Table
        modelBuilder.Entity<Context>(entity =>
        {
            entity.ToTable("Contexts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // --- PARENT CONFIGURATION (DashboardItem) ---
        modelBuilder.Entity<DashboardItem>(entity =>
        {
            // Table-Per-Type (TPT): This is the main table
            entity.ToTable("DashboardItems");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(1000);

            // Enum Mapping: Store Enums as Strings in the DB (Readable!)
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(50);

            // Storing the List<ExternalLink> as a JSON document
            entity.Property(e => e.ExternalLinks).HasColumnType("jsonb");

            // Relationship setup
            entity.HasOne(d => d.Context)
                  .WithMany(c => c.Items)
                  .HasForeignKey(d => d.ContextId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // --- CHILD CONFIGURATION (Anime) ---
        modelBuilder.Entity<Anime>(entity =>
        {
            // Table-Per-Type (TPT): This table shares the SAME ID as DashboardItems
            entity.ToTable("Animes");

            // Database Constraints for data integrity
            entity.Property(e => e.TotalEpisodes).HasDefaultValue(0);
            entity.Property(e => e.CurrentEpisodes).HasDefaultValue(0);
            entity.Property(e => e.LastWatchedEpisode).HasDefaultValue(0);
            entity.Property(e => e.AiringStatus).HasMaxLength(50);

            // JSONB MAPPING: Storing List<string> genres as a JSON document
            entity.Property(e => e.Genres).HasColumnType("jsonb");
        });
    }
}
