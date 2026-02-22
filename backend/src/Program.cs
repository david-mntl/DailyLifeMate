using DailyLifeMate.Api.Extensions;
using DailyLifeMate.Engine.Configure;
using DailyLifeMate.Infrastructure.Database;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Npgsql;

// Using new standard way to init a program (All in program.cs as a script file)
var builder = WebApplication.CreateBuilder(args);

// --- Registering app services ---
builder.Services.AddEngineServices();
builder.Services.AddRepositories();
builder.Services.AddProviders(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------------

// Setting up DB (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("Postgres");

// Create the DataSource with JSON enabled
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

// Tells EF Core to use that DataSource
builder.Services.AddDbContext<DailyLifeMateDbContext>(options =>
{
    options.UseNpgsql(dataSource);
});


// Running the program
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
// app.UseAuthorization(); // Future to use AUTH
app.MapControllers();

// Initialize the database
app.SeedDatabase();

app.Run();

// This makes the Program class public so tests can reference it
public partial class Program { }
