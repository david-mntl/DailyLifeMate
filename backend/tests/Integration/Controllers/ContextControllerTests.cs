using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;

using DailyLifeMate.Domain.Core.Models;
using DailyLifeMate.Engine.Common.Contexts.Dtos;
using DailyLifeMate.Engine.Features.Series.Models;
using DailyLifeMate.Tests.Integration.Common;


using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

namespace DailyLifeMate.Tests.Integration.Controllers;

public class ContextsControllerTests : DBSetupFixtureBase
{
    private const string BaseUrl = "/api/context";

    #region CreateAsync Tests

    [Test]
    public async Task CreateAsync_WithValidRequest_ReturnsCreatedStatusAndCreatesRecord()
    {
        // Arrange
        var request = new CreateContextRequestDto
        {
            Name = "Test Context",
            Description = "A context used for testing."
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
        var content = await response.Content.ReadFromJsonAsync<ContextDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verify Response Content
        content.Should().NotBeNull();
        content!.Name.Should().Be(request.Name);
        content.Description.Should().Be(request.Description);
        content.IsArchived.Should().BeFalse(); // Ensure default mapping worked

        // Verify Record in Database
        var contextInDb = await _dbContext.Contexts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == content.Id);

        contextInDb.Should().NotBeNull();
        contextInDb!.Name.Should().Be(request.Name);
        contextInDb.Description.Should().Be(request.Description);
        contextInDb.IsArchived.Should().BeFalse();
    }

    [Test]
    public async Task CreateAsync_WithMissingName_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            Description = "TestDescription Without Name"
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region GetByIdAsync Tests

    [Test]
    public async Task GetByIdAsync_WithValidId_ReturnsOkAndContextData()
    {
        // Arrange
        var context = new Context
        {
            Id = Guid.NewGuid(),
            Name = "Gaming",
            Description = "Video games context",
            IsArchived = false
        };

        _dbContext.Contexts.Add(context);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _httpClient.GetAsync($"{BaseUrl}/{context.Id}");
        var content = await response.Content.ReadFromJsonAsync<ContextDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content!.Id.Should().Be(context.Id);
        content.Name.Should().Be(context.Name);
    }

    [Test]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Act
        var response = await _httpClient.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test] // CHECK
    public async Task GetByIdAsync_WhenContextHasItems_ReturnsContextWithMappedItems()
    {
        // Arrange
        var contextId = Guid.NewGuid();
        var context = new Context
        {
            Id = contextId,
            Name = "Watchlist Context",
            Description = "Contains my shows",
            IsArchived = false
        };

        var anime = new Anime
        {
            Id = Guid.NewGuid(),
            Name = "Attack on Titan",
            Description = "Giant humanity-eating monsters.",
            ContextId = contextId
        };

        _dbContext.Contexts.Add(context);
        _dbContext.Animes.Add(anime);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _httpClient.GetAsync($"{BaseUrl}/{contextId}");
        var content = await response.Content.ReadFromJsonAsync<ContextDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();

        content!.Id.Should().Be(contextId);

        // Verify the Items list successfully pulled the Anime!
        content.Items.Should().NotBeNull();
        content.Items.Should().HaveCount(1);

        DashboardItemSummaryDto mappedItem = content.Items.First();
        mappedItem.Id.Should().Be(anime.Id);
        mappedItem.Name.Should().Be(anime.Name);
        mappedItem.Description.Should().Be(anime.Description);
    }

    #endregion

    #region GetAllAsync Tests

    [Test]
    public async Task GetAllAsync_ReturnsOkWithAllContexts()
    {
        // Arrange
        var context1 = new Context { Id = Guid.NewGuid(), Name = "Movies", IsArchived = false };
        var context2 = new Context { Id = Guid.NewGuid(), Name = "Books", IsArchived = true };

        _dbContext.Contexts.AddRange(context1, context2);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _httpClient.GetAsync(BaseUrl);
        var content = await response.Content.ReadFromJsonAsync<List<ContextDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        // Check for contain instead of exact count, because the DBInitializer might seed contexts too
        content.Should().Contain(c => c.Name == "Movies");
        content.Should().Contain(c => c.Name == "Books" && c.IsArchived == true);
    }

    #endregion

    #region UpdateAsync Tests

    [Test]
    public async Task UpdateAsync_WithValidRequest_ReturnsOkAndUpdatesRecord()
    {
        // Arrange
        var originalContext = new Context
        {
            Id = Guid.NewGuid(),
            Name = "Original Name",
            Description = "Original Description",
            IsArchived = false
        };

        _dbContext.Contexts.Add(originalContext);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(originalContext).State = EntityState.Detached;

        var updateRequest = new UpdateContextRequestDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            IsArchived = true
        };

        // Act
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{originalContext.Id}", updateRequest);
        var content = await response.Content.ReadFromJsonAsync<ContextDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Check Response
        content!.Name.Should().Be(updateRequest.Name);
        content.Description.Should().Be(updateRequest.Description);
        content.IsArchived.Should().BeTrue();

        // Check Database
        var contextInDb = await _dbContext.Contexts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == originalContext.Id);

        contextInDb.Should().NotBeNull();
        contextInDb!.Name.Should().Be(updateRequest.Name);
        contextInDb.Description.Should().Be(updateRequest.Description);
        contextInDb.IsArchived.Should().BeTrue();
    }

    [Test]
    public async Task UpdateAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var updateRequest = new UpdateContextRequestDto { Name = "Updated", IsArchived = false };

        // Act
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{Guid.NewGuid()}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region DeleteAsync Tests

    [Test]
    public async Task DeleteAsync_WithValidId_ReturnsNoContentAndDeletesRecord()
    {
        // Arrange
        var context = new Context
        {
            Id = Guid.NewGuid(),
            Name = "To Be Deleted",
            IsArchived = false
        };

        _dbContext.Contexts.Add(context);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(context).State = EntityState.Detached;

        // Act
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{context.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify Deletion
        var contextInDb = await _dbContext.Contexts.FirstOrDefaultAsync(c => c.Id == context.Id);
        contextInDb.Should().BeNull();
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
}