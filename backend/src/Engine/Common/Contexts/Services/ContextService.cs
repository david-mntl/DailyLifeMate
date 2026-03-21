using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DailyLifeMate.Domain.Core.Exceptions;
using DailyLifeMate.Domain.Core.Models;
using DailyLifeMate.Domain.Persistence;
using DailyLifeMate.Engine.Common.Contexts.Dtos;

using Microsoft.Extensions.Logging;

namespace DailyLifeMate.Engine.Common.Contexts.Services;

public class ContextService : IContextService
{
    private readonly IRepository<Context> _contextRepository;
    private readonly ILogger<ContextService> _logger;

    public ContextService(
        IRepository<Context> contextRepository,
        ILogger<ContextService> logger)
    {
        _contextRepository = contextRepository;
        _logger = logger;
    }

    public async Task<List<ContextDto>> GetAllAsync()
    {
        _logger.LogDebug("Fetching all contexts from the database.");
        var contexts = await _contextRepository.GetAllAsync();
        return contexts.Select(MapToDto).ToList();
    }

    public async Task<ContextDto> GetByIdAsync(Guid id)
    {
        var context = await GetContextOrThrowAsync(id);
        return MapToDto(context);
    }

    public async Task<ContextDto> CreateAsync(CreateContextRequestDto request)
    {
        _logger.LogDebug("Trying to create context {Name}", request.Name);
        var context = new Context
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            IsArchived = false
        };

        _contextRepository.Add(context);
        await _contextRepository.SaveChangesAsync();

        _logger.LogInformation("Created new Context: {Name} ({Id})", context.Name, context.Id);

        return MapToDto(context);
    }

    public async Task<ContextDto> UpdateAsync(Guid id, UpdateContextRequestDto request)
    {
        _logger.LogDebug("Trying to update context {Name}", request.Name);
        var context = await GetContextOrThrowAsync(id, request.Name);

        context.Name = request.Name;
        context.Description = request.Description;
        context.IsArchived = request.IsArchived;

        _contextRepository.Update(context);
        await _contextRepository.SaveChangesAsync();

        _logger.LogInformation("Updated Context: {Id}", id);

        return MapToDto(context);
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug("Trying to delete context {Id}", id);
        var context = await GetContextOrThrowAsync(id);

        await _contextRepository.DeleteAsync(context.Id);
        await _contextRepository.SaveChangesAsync();

        _logger.LogInformation("Deleted Context: {Id}", id);
    }

    // --- Centralized error handling ---
    private async Task<Context> GetContextOrThrowAsync(Guid id, string? name = null)
    {
        var context = await _contextRepository.GetByIdAsync(id, c => c.Items);
        if (context == null)
        {
            _logger.LogWarning("Context with ID {Id} was not found.", id);

            if (!string.IsNullOrWhiteSpace(name))
            {
                throw new NotFoundException(nameof(Context), id, name);
            }

            throw new NotFoundException(nameof(Context), id);
        }
        return context;
    }

    private static ContextDto MapToDto(Context context)
    {
        var itemDtos = context.Items?
            .Select(i => new DashboardItemSummaryDto(
                i.Id,
                i.Name,
                i.Description ?? string.Empty))
            .ToList() ?? new List<DashboardItemSummaryDto>();

        return new ContextDto(
            context.Id,
            context.Name,
            context.Description ?? string.Empty,
            context.IsArchived,
            itemDtos);
    }
}