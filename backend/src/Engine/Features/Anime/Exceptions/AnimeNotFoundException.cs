using System;

namespace DailyLifeMate.Domain.Exceptions;

public class AnimeNotFoundException : Exception
{
    public AnimeNotFoundException(Guid id)
        : base($"Anime with ID {id} was not found.") { }
}