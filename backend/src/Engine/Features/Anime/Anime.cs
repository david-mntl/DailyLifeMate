namespace DailyLifeMate.Domain.Features.Anime;

public class Anime
{
    private readonly Item _innerItem;

    public Anime(Item item)
    {
        if (item.Context.Type != ContextType.Anime)
            throw new ArgumentException("Item does not belong to context: Anime.");
        
        _innerItem = item;
    }

    // Properties pull from the Metadata or the Item itself
    public Guid Id => _innerItem.Id;
    public string Title => _innerItem.Title;

    // Type-safe accessors
    public DateTime? NextAirDateUtc 
    {
        get
        {
            var rawValue = GetMetadata(AnimeMetadataKeys.NextAirDate);
            if (DateTime.TryParse(rawValue, null, DateTimeStyles.RoundtripKind, out var date))
            {
                return date;
            }
            return null;
        }
    }

    // This is what the UI (or Alexa) will use
    public DateTime? NextAirDateLocal => NextAirDateUtc?.ToLocalTime();

    // Readable helper for your dashboard
    public string LocalAirDateDisplay => 
        NextAirDateLocal?.ToString("f") ?? "TBA"; // "f" is full date/short time

    public string? WatchUrl => GetMetadata(AnimeMetadataKeys.WatchUrl); // May need to be changed

    public int CurrentEpisode => 
        int.TryParse(GetMetadata(AnimeMetadataKeys.CurrentEpisode), out var val)

    // Internal helper to handle the Enum-to-String lookup
    private string? GetMetadata(AnimeMetadataKeys key)
    {
        return _innerItem.Metadata
            .FirstOrDefault(m => m.Key == key.ToString())?.Value;
    }
    
    // public string? NextEpisodeDate => 
    //     _innerItem.Metadata.FirstOrDefault(m => m.Key == "NextAirDate")?.Value;

    // public int CurrentEpisode => 
    //     int.Parse(_innerItem.Metadata.FirstOrDefault(m => m.Key == "CurrentEpisode")?.Value ?? "0");

    // Helper logic specific to Anime
    // public bool IsNewEpisodeAvailable() 
    // {
    //     if (DateTime.TryParse(NextEpisodeDate, out var airDate))
    //     {
    //         return DateTime.UtcNow >= airDate;
    //     }
    //     return false;
    // }
}