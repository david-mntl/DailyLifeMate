namespace DailyLifeMate.Infrastructure.ExternalApis.Jikan.Models;

public record JikanApiSettings
{
    public const string SectionName = "ExternalApis:Jikan";

    public string BaseUrl { get; set; } = string.Empty;
    public string UserAgent { get; set; } = "DailyLifeMate/1.0";
}
