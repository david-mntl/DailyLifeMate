using System;

namespace DailyLifeMate.Engine.Features.Series.Exceptions;

public class AnimeNotFoundException : Exception
{
    public AnimeNotFoundException(Guid id)
        : base($"Anime with ID {id} was not found.") { }
}
