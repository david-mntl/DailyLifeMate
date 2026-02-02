namespace DailyLifeMate.Infrastructure.Database;

using DailyLifeMate.Domain.Core.Enums;
using DailyLifeMate.Domain.Core.Models;

public static class DbInitializer
{
    public static void Seed(DailyLifeMateDbContext context)
    {
        // Ensure the database is created
        context.Database.EnsureCreated();

        // Check if the Anime context already exists
        if (!context.Contexts.Any(c => c.Type == ContextType.Anime))
        {
            context.Contexts.Add(new Context
            {
                Id = Guid.NewGuid(),
                Type = ContextType.Anime,
                Name = "Anime Dashboard",
                Description = "Context for tracking anime that is being watched.",
                CreatedAt = DateTime.UtcNow
            });

            context.SaveChanges();
        }
    }
}