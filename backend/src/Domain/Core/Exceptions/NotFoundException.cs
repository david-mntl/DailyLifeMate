using System;

namespace DailyLifeMate.Domain.Core.Exceptions;

public class NotFoundException : Exception
{
    // Constructor for ID requests (e.g. standard GetById lookups)
    public NotFoundException(string resourceName, Guid key)
        : base($"{resourceName} with ID '{key}' was not found.")
    {
    }
    public NotFoundException(string resourceName, Guid key, string name)
        : base($"{resourceName} with ID '{key}' and name '{name}' was not found.")
    {
    }
    public NotFoundException(string resourceName, string name)
        : base($"{resourceName} with name '{name}' was not found.")
    {
    }
}