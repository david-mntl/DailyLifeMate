using DailyLifeMate.Engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DailyLifeMate.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using DailyLifeMate.Api.Extensions;

// Using new standard way to init a program (All in program.cs as a script file)
var builder = WebApplication.CreateBuilder(args);

// Framework services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- Registering app services ---
builder.Services.AddEngineServices();
// --------------------------

// Setting up DB (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DailyLifeMateDbContext>(options =>
    options.UseNpgsql(connectionString));

// Running the program
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Standard practice to force HTTPS
// app.UseAuthorization(); // Future to use AUTH
app.MapControllers();

// Initialize the database
app.SeedDatabase();

app.Run();