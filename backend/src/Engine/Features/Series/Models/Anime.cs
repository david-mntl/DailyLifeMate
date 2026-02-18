using System;
using System.Collections.Generic;

using DailyLifeMate.Domain.Core.Enums;
using DailyLifeMate.Domain.Core.Models;

namespace DailyLifeMate.Engine.Features.Series.Models;

public class Anime : DashboardItem
{
    public Anime()
    {
        Type = ItemType.Anime;
    }

    public DateTime? ReleasedOn { get; set; }
    public DateTime? NextAirDateUtc { get; set; }

    public uint TotalEpisodes { get; set; }
    public uint CurrentEpisodes { get; set; }
    public uint LastWatchedEpisode { get; set; }

    public string AiringStatus { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new();
}
