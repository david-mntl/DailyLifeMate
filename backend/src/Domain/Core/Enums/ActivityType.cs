namespace DailyLifeMate.Domain.Core.Enums;

public enum ActivityType
{
    Opened,     // User clicked the link
    Started,    // Intent to watch/do
    Completed,  // Finished the episode/session
    Skipped,    // Marked as not interested
    Scheduled   // Future intent (e.g., "Remind me when this airs")
}