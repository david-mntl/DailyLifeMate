
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DailyLifeMate.Engine.Features.Series.Dtos;
using DailyLifeMate.Engine.Features.Series.Exceptions;
using DailyLifeMate.Engine.Features.Series.Services;

using Microsoft.AspNetCore.Mvc;

namespace DailyLifeMate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimeController : ControllerBase
{
    private readonly IAnimeService _animeService;

    public AnimeController(IAnimeService animeService)
    {
        _animeService = animeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AnimeDto>>> GetAll()
    {
        try
        {
            var result = await _animeService.GetAllAnimesAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<AnimeDto>> Create([FromBody] CreateAnimeRequestDto request)
    {
        try
        {
            var result = await _animeService.CreateAnimeAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An internal error occurred.", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AnimeDto>> GetById(Guid id)
    {
        try
        {
            var result = await _animeService.GetAnimeByIdAsync(id);
            return Ok(result);
        }
        catch (AnimeNotFoundException ex)
        {
            // Return 404 Not Found
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Internal Server Error
            return StatusCode(500, new { message = "A server error occurred.", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AnimeDto>> Update(Guid id, [FromBody] UpdateAnimeRequestDto request)
    {
        try
        {
            var result = await _animeService.UpdateAnimeAsync(id, request);
            return Ok(result);
        }
        catch (AnimeNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await _animeService.DeleteAnimeAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Anime not found." });
            }

            return NoContent(); // 204 No Content is the standard for successful deletes
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
