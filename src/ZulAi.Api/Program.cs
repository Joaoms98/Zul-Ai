using Microsoft.EntityFrameworkCore;
using ZulAi.Application.Interfaces;
using ZulAi.Application.Rules;
using ZulAi.Application.Services;
using ZulAi.Domain.Interfaces;
using ZulAi.Infrastructure.Data;
using ZulAi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// MySQL via Pomelo
var connectionString = builder.Configuration.GetConnectionString("ZulAiDb");
var serverVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<ZulAiDbContext>(options =>
    options.UseMySql(connectionString, serverVersion, mysqlOptions =>
    {
        mysqlOptions.MigrationsAssembly("ZulAi.Infrastructure");
    }));

// Repositories
builder.Services.AddScoped<IUniverseStateRepository, UniverseStateRepository>();
builder.Services.AddScoped<IAtomRepository, AtomRepository>();
builder.Services.AddScoped<IConnectionRepository, ConnectionRepository>();
builder.Services.AddScoped<IInteractionRepository, InteractionRepository>();
builder.Services.AddScoped<IGeneratedOutputRepository, GeneratedOutputRepository>();

// Application services
builder.Services.AddSingleton<IAtomFactory, AtomFactory>();
builder.Services.AddSingleton<IAsciiRenderer, AsciiRenderer>();
builder.Services.AddSingleton<IConnectionRule, ProximityRule>();
builder.Services.AddSingleton<IConnectionRule, EnergyCompatibilityRule>();
builder.Services.AddSingleton<IConnectionRule, TypeAffinityRule>();
builder.Services.AddScoped<IConnectionRuleEngine, ConnectionRuleEngine>();
builder.Services.AddScoped<IUniverseService, UniverseService>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("ReactApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
