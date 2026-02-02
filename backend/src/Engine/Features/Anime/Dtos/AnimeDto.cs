namespace DailyLifeMate.Domain.Dtos;

public class AnimeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Rating { get; set; }
}