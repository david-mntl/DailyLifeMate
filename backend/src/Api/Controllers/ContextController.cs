using System;
using System.Threading.Tasks;

using DailyLifeMate.Domain.Core.Exceptions;
using DailyLifeMate.Engine.Common.Contexts.Dtos;
using DailyLifeMate.Engine.Common.Contexts.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DailyLifeMate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContextController : ControllerBase
{
    private readonly IContextService _contextService;
    private readonly ILogger<ContextController> _logger;

    public ContextController(IContextService contextService, ILogger<ContextController> logger)
    {
        _contextService = contextService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var contexts = await _contextService.GetAllAsync();
            return Ok(contexts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching all contexts.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred.", details = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var context = await _contextService.GetByIdAsync(id);
            return Ok(context);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch context with ID {Id}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred.", details = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContextRequestDto request)
    {
        try
        {
            // ADD VALIDATOR
            var createdContext = await _contextService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = createdContext.Id }, createdContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create context: {Name}.", request.Name);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred.", details = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContextRequestDto request)
    {
        try
        {
            var updatedContext = await _contextService.UpdateAsync(id, request);
            return Ok(updatedContext);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update context with ID {Id}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred.", details = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _contextService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete context with ID {Id}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred.", details = ex.Message });
        }
    }
}