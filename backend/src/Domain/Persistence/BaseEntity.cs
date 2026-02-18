
using System;

namespace DailyLifeMate.Domain.Persistence;

public abstract class BaseEntity : IEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public string? Description { get; set; } = string.Empty;
}