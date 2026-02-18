using System;

namespace DailyLifeMate.Domain.Core.Models;

public class ExternalLink
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Priority { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DashboardItem Item { get; set; } = null!;
}
