using DailyLifeMate.Domain.Persistence;

namespace DailyLifeMate.Domain.Core.Models;

public abstract class BaseItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsArchived { get; set; } = false;
}