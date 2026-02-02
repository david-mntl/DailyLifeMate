namespace DailyLifeMate.Domain.Core.Models;

public class ExternalLink
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }

    public string Title { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int Priority { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Item Item { get; set; } = null!;
}