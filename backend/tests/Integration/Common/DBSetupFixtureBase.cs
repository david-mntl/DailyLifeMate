using System.Net.Http;
using System.Threading.Tasks;

using DailyLifeMate.Infrastructure.Database;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace DailyLifeMate.Tests.Integration.Common;

public abstract class DBSetupFixtureBase
{
    protected TestServer _testServer = null!;
    protected HttpClient _httpClient = null!;
    protected DailyLifeMateDbContext _dbContext = null!;


    [OneTimeSetUp]
    public async Task BaseOneTimeSetUpAsync()
    {
        _testServer = new TestServer();
        await _testServer.InitializeAsync();
    }

    [SetUp]
    public async Task BaseSetUpAsync()
    {
        _httpClient = _testServer.CreateClient();

        // Get Context
        var scope = _testServer.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<DailyLifeMateDbContext>();
        await _dbContext.Database.EnsureCreatedAsync();

        await _testServer.ResetDatabaseAsync();
    }

    [TearDown]
    public async Task BaseTearDownAsync()
    {
        // Dispose Context
        if (_dbContext != null)
        {
            await _dbContext.DisposeAsync();
        }

        // Dispose Client
        _httpClient?.Dispose();
    }

    [OneTimeTearDown]
    public async Task BaseOneTimeTearDownAsync()
    {
        if (_testServer != null)
        {
            await _testServer.DisposeAsync();
            _testServer.Dispose();
        }
    }
}
