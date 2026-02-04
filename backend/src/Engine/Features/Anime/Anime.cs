using System;
using System.Collections.Generic;
using DailyLifeMate.Domain.Core.Models;

namespace DailyLifeMate.Domain.Features.Anime;

public class Anime : DashboardItem
{
    public Anime()
    {
        base.Type = ItemType.Anime; // Always sets itself correctly
    }

    public DateTime? ReleasedOn { get; set; }
    public DateTime? NextAirDateUtc { get; set; }

    public int TotalEpisodes { get; set; }
    public int CurrentEpisodes { get; set; }
    public int LastWatchedEpisode { get; set; }

    public string AiringStatus { get; set; } = "Planned";
    public List<string> Genres { get; set; } = new();
}