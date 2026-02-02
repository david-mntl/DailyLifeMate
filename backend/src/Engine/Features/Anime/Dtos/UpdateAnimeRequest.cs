namespace DailyLifeMate.Domain.Dtos;

public class UpdateAnimeRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Rating { get; set; } // New field specifically for updates
}