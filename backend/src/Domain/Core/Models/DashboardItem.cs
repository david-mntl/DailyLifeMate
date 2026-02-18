using System;
using System.Collections.Generic;

using DailyLifeMate.Domain.Core.Enums;

namespace DailyLifeMate.Domain.Core.Models;

public abstract class DashboardItem : BaseItem
{
    public ActivityStatus Status { get; set; }
    public ItemType Type { get; set; }
    public List<ExternalLink> ExternalLinks { get; set; } = new();

    // The relationship: Every item must live inside a Context/Dashboard
    public Guid ContextId { get; set; }
    public Context Context { get; set; } = null!;
}
