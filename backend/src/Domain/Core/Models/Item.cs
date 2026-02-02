namespace DailyLifeMate.Domain.Core.Models;

public class Item
{
    public Guid Id { get; set; }
    public Guid ContextId { get; set; }

    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Context Context { get; set; } = null!;
    public ICollection<ItemActivity> Activity { get; set; } = new List<ItemActivity>();
    public ICollection<ItemMetadata> Metadata { get; set; } = new List<ItemMetadata>();
    public ICollection<ExternalLink> ExternalLinks { get; set; } = new List<ExternalLink>();
}

public class Item
{
    public Guid Id { get; set; }
    public Guid ContextId { get; set; }
    
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? ExternalId { get; set; } //Maybe dont need it
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Context Context { get; set; } = null!;
    public ICollection<ItemMetadata> Metadata { get; set; } = new List<ItemMetadata>();
    public ICollection<ExternalLink> ExternalLinks { get; set; } = new List<ExternalLink>();
    
    // The Behavior Bridge
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}