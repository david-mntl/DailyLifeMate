using System;
using System.Data.Common;
using System.Threading.Tasks;

using DailyLifeMate.Infrastructure.Database;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using Respawn;

namespace DailyLifeMate.Tests.Integration.Common;

public class TestServer : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private DbConnection? _dbConnection;
    private Respawner? _respawner;

    public TestServer()
    {
        _connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres") ?? "";
    }

    public async Task InitializeAsync()
    {
        // Services triggers the host build
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DailyLifeMateDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        _dbConnection = new NpgsqlConnection(_connectionString);
        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" }
            // TablesToIgnore = new Table[] { "Context" }
        });
    }

    public async Task ResetDatabaseAsync()
    {
        if (_respawner == null || _dbConnection == null)
        {
            throw new InvalidOperationException("TestServer not initialized. Call InitializeAsync first.");
        }


        await _respawner.ResetAsync(_dbConnection);

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DailyLifeMateDbContext>();

        DbInitializer.Initialize(dbContext);

    }

    public override async ValueTask DisposeAsync()
    {
        if (_dbConnection != null)
        {
            await _dbConnection.CloseAsync();
            _dbConnection.Dispose();
        }
    }
}
