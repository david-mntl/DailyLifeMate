namespace DailyLifeMate.Domain.Dtos;

public class CreateAnimeRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}