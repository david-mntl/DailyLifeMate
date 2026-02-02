using DailyLifeMate.Engine;
using DailyLifeMate.Database;
using Microsoft.EntityFrameworkCore;

// Using new standard way to init a program (All in program.cs as a script file)
var builder = WebApplication.CreateBuilder(args);

// Framework services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Registering app services ---
builder.Services.AddEngineServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
// --------------------------

// Setting up DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DailyLifeMateDbContext>(options =>
    options.UseNpgsql(connectionString));

// Running the program
var app = builder.Build();

// 3. Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();