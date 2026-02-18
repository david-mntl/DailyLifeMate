
using System;

namespace DailyLifeMate.Domain.Persistence;

public interface IEntity
{
    public Guid Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; set; }
    public string? Description { get; set; }
}