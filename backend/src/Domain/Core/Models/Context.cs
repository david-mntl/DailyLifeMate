namespace DailyLifeMate.Domain.Core.Models;

public class Context
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public ContextType Type { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public User User { get; set; } = null!;
    public List<Item> Items { get; set; } = new List<Item>();
}