using System.Collections.Generic;
using DailyLifeMate.Domain.Core.Models;

namespace DailyLifeMate.Domain.Core;

public class Context
{
    public System.Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigation property: One context can hold many different items
    public List<DashboardItem> Items { get; set; } = new();
}