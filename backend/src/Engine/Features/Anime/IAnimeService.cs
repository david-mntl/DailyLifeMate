using DailyLifeMate.Database;
using DailyLifeMate.Domain.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DailyLifeMate.Engine.Services;

public interface IAnimeService 
{
    Task<List<AnimeDto>> GetAllAnimesAsync();
    Task<AnimeDto> CreateAnimeAsync(CreateAnimeRequest request);
}