namespace DailyLifeMate.Domain.Core;

public class ItemMetadata
{
    public Guid Id { get; set; } // objectId

    // ID to the Parent Item
    public Guid ItemId { get; set; }

    // Navigation Property: Object itself
    public Item Item { get; set; } = null!;

    // Stored this as a string to remain generic across all contexts.
    public string Key { get; set; } = null!;

    // The actual data stored as a string.   
    // parsing this into Dates, Integers, or Booleans.
    public string? Value { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }


}