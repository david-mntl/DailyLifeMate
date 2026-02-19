using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;

using DailyLifeMate.Domain.Core.Models;
using DailyLifeMate.Engine.Features.Series.Dtos;
using DailyLifeMate.Engine.Features.Series.Models;
using DailyLifeMate.Infrastructure.Database;
using DailyLifeMate.Tests.Integration.Common;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

namespace DailyLifeMate.Tests.Integration.Controllers;

public class AnimeControllerTests : DBSetupFixtureBase
{
    private const string BaseUrl = "/api/anime";
    private const string AnimeContextName = DbInitializer.SEED_CONTEXT_NAME; // Ensure this string matches DbInitializer exactly

    #region CreateAsync Tests

    [Test]
    public async Task CreateAsync_WithValidRequest_ReturnsCreatedStatusAndCreatesRecord()
    {
        // Arrange
        var request = new CreateAnimeRequestDto
        {
            Name = "Test Anime",
            Description = "A test anime description",
            TotalEpisodes = 12
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
        var content = await response.Content.ReadFromJsonAsync<AnimeDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verify Response Content
        content.Should().NotBeNull();
        content!.Name.Should().Be(request.Name);
        content.Description.Should().Be(request.Description);
        content.TotalEpisodes.Should().Be(request.TotalEpisodes);
        content.ContextName.Should().Be(AnimeContextName); // Verified via the new Service logic

        // Verify Record in Database
        // AsNoTracking to ensure to get fresh data from DB, not cache
        var animeInDb = await _dbContext.Animes
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == content.Id);

        animeInDb.Should().NotBeNull();
        animeInDb!.Name.Should().Be(request.Name);
        animeInDb.TotalEpisodes.Should().Be(12);
    }

    [Test]
    public async Task CreateAsync_WithMissingName_ReturnsBadRequest()
    {
        // Arrange
        // Anonymos object to simulate missing property 'Name'.
        var request = new
        {
            Description = "TestDescription",
            TotalEpisodes = 4,
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Verify no record was created (count should be 0)
        var count = await _dbContext.Animes.CountAsync();
        count.Should().Be(0);
    }

    #endregion

    #region GetByIdAsync Tests

    [Test]
    public async Task GetByIdAsync_WithValidId_ReturnsOkAndAnimeData()
    {
        // Arrange
        var animeContext = await GetSeedContextAsync();

        var anime = new Anime
        {
            Id = Guid.NewGuid(),
            ContextId = animeContext.Id,
            Name = "Hunter X Hunter",
            Description = "Hunter anime",
            TotalEpisodes = 148,
            CurrentEpisodes = 148,
            ExternalLinks = new List<ExternalLink>()
        };

        _dbContext.Animes.Add(anime);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _httpClient.GetAsync($"{BaseUrl}/{anime.Id}");
        var content = await response.Content.ReadFromJsonAsync<AnimeDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content!.Id.Should().Be(anime.Id);
        content.Name.Should().Be(anime.Name);
        content.ContextName.Should().Be(AnimeContextName);
    }

    [Test]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();

        // Act
        var response = await _httpClient.GetAsync($"{BaseUrl}/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetByIdAsync_WithInvalidId_ReturnsBadRequest()
    {
        // Act
        // Sending "99999" (int) to a route expecting {id:Guid} fails Model Binding
        var response = await _httpClient.GetAsync($"{BaseUrl}/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region GetAllAsync Tests

    [Test]
    public async Task GetAllAsync_ReturnsOkWithAllAnimes()
    {
        // Arrange
        var animeContext = await GetSeedContextAsync();

        var anime1 = new Anime
        {
            Id = Guid.NewGuid(),
            ContextId = animeContext.Id,
            Name = "Attack on Titan",
            Description = "Titans",
            TotalEpisodes = 25,
            ExternalLinks = []
        };
        var anime2 = new Anime
        {
            Id = Guid.NewGuid(),
            ContextId = animeContext.Id,
            Name = "Demon Slayer",
            Description = "Demons",
            TotalEpisodes = 26,
            ExternalLinks = []
        };

        _dbContext.Animes.AddRange(anime1, anime2);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _httpClient.GetAsync(BaseUrl);
        var content = await response.Content.ReadFromJsonAsync<List<AnimeDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().HaveCount(2);
        content.Should().Contain(a => a.Name == "Attack on Titan");
        content.Should().Contain(a => a.Name == "Demon Slayer");
    }

    #endregion

    #region UpdateAsync Tests

    [Test]
    public async Task UpdateAsync_WithValidRequest_ReturnsOkAndUpdatesRecord()
    {
        // Arrange
        var animeContext = await GetSeedContextAsync();

        var originalAnime = new Anime
        {
            Id = Guid.NewGuid(),
            ContextId = animeContext.Id,
            Name = "Original Name",
            Description = "Original Description",
            TotalEpisodes = 12,
            ExternalLinks = []
        };

        _dbContext.Animes.Add(originalAnime);
        await _dbContext.SaveChangesAsync();

        // Detach the entity so EF doesn't track it locally (ensures assert fetches fresh data)
        _dbContext.Entry(originalAnime).State = EntityState.Detached;

        var updateRequest = new UpdateAnimeRequestDto
        {
            Name = "Updated Name",
            Description = "Updated Description"
        };

        // Act
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{originalAnime.Id}", updateRequest);
        var content = await response.Content.ReadFromJsonAsync<AnimeDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Check Response
        content!.Name.Should().Be(updateRequest.Name);
        content.Description.Should().Be(updateRequest.Description);

        // Check Database
        var animeInDb = await _dbContext.Animes
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == originalAnime.Id);

        animeInDb.Should().NotBeNull();
        animeInDb!.Name.Should().Be("Updated Name");
        animeInDb.Description.Should().Be("Updated Description");
    }

    [Test]
    public async Task UpdateAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();
        var updateRequest = new UpdateAnimeRequestDto
        {
            Name = "Updated Name",
            Description = "Updated Description"
        };

        // Act
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{nonExistentId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region DeleteAsync Tests

    [Test]
    public async Task DeleteAsync_WithValidId_ReturnsNoContentAndDeletesRecord()
    {
        // Arrange
        var animeContext = await GetSeedContextAsync();

        var anime = new Anime
        {
            Id = Guid.NewGuid(),
            ContextId = animeContext.Id,
            Name = "To Be Deleted",
            ExternalLinks = []
        };

        _dbContext.Animes.Add(anime);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(anime).State = EntityState.Detached;

        // Act
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{anime.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify Deletion
        var animeInDb = await _dbContext.Animes.FirstOrDefaultAsync(a => a.Id == anime.Id);
        animeInDb.Should().BeNull();
    }

    [Test]
    public async Task DeleteAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Act
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    // Helper to ensure tests fail clearly if seed data is missing
    private async Task<Context> GetSeedContextAsync()
    {
        var context = await _dbContext.Contexts.FirstOrDefaultAsync(c => c.Name == AnimeContextName);
        if (context == null)
        {
            Assert.Fail($"The Seed Context '{AnimeContextName}' was not found. Check DbInitializer.");
        }
        return context!;
    }
}
