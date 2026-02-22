using System.Collections.Generic;

namespace DailyLifeMate.Engine.Features.Series.Models;

public class AnimeMetadata
{
    public string Title { get; set; } = string.Empty;
    public string Synopsis { get; set; } = string.Empty;
    public int? TotalEpisodes { get; set; }
    public string Status { get; set; } = string.Empty;
    public System.DateTime? ReleasedOn { get; set; }
    public List<string> Genres { get; set; } = [];
    public string? ImageUrl { get; set; }
}
