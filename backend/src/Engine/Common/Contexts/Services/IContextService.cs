using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DailyLifeMate.Engine.Common.Contexts.Dtos;

namespace DailyLifeMate.Engine.Common.Contexts.Services;

public interface IContextService
{
    Task<List<ContextDto>> GetAllAsync();
    Task<ContextDto> GetByIdAsync(Guid id);
    Task<ContextDto> CreateAsync(CreateContextRequestDto request);
    Task<ContextDto> UpdateAsync(Guid id, UpdateContextRequestDto request);
    Task DeleteAsync(Guid id);
}