namespace DailyLifeMate.Domain.Core.Models;

public class Activity
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    
    public ActivityType Type { get; set; }
    public DateTime Timestamp { get; set; }
    
    // For context-specific behavior data (e.g., "watched_duration": 1200)
    public string? PropertiesJson { get; set; }

    // Navigation
    public Item Item { get; set; } = null!;
}