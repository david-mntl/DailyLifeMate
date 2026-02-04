using System;
using System.Collections.Generic;
using DailyLifeMate.Domain.Core.Enums;

namespace DailyLifeMate.Domain.Core.Models;

public abstract class DashboardItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsArchived { get; set; } = false;
    public ActivityStatus Status { get; set; }
    public ItemType Type { get; set; }
    public List<ExternalLink> ExternalLinks { get; set; } = new();

    // The relationship: Every item must live inside a Context/Dashboard
    public Guid ContextId { get; set; }
    public Context Context { get; set; } = null!;
}