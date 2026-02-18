using System.Collections.Generic;

namespace DailyLifeMate.Domain.Core.Models;

public class Context : BaseItem
{
    // Navigation property: One context can hold many different items
    public List<DashboardItem> Items { get; set; } = new();
}
