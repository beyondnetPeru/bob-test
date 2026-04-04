using Application;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Resilience;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Middleware;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configuration & Dependency Injection ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Issue #9: Swagger metadata
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FleetOps Hardware Inventory API",
        Version = "v1",
        Description = "Enterprise-grade hardware asset management API built with Clean Architecture, CQRS, and the Result pattern."
    });
});

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

// Infrastructure Layer — Repositories & Unit of Work
builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IAssetConfigurationRepository, AssetConfigurationRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// Infrastructure Layer — Services (abstracted behind interfaces for DIP)
builder.Services.AddScoped<ICleansingService, CleansingService>();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddApiResilience(builder.Configuration);

// Application Layer (MediatR, AutoMapper, FluentValidation)
builder.Services.AddApplication();

// --- 2. Security: CORS, Health Checks ---
// Issue #11: Scoped CORS — explicit methods and headers
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:4200")
            .WithMethods("GET", "POST", "PUT", "DELETE")
            .WithHeaders("Content-Type", "Authorization");
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

// --- 3. Middleware Pipeline ---
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FleetOps API V1");
    c.RoutePrefix = "swagger";
});

app.UseCors();
app.MapHealthChecks("/health");

// --- 4. Feature Controllers ---
app.MapControllers();

// Issue #10: Database initialization
// WARNING: EnsureCreated() does NOT run EF Core migrations.
// For production, replace with db.Database.MigrateAsync() and a proper migrations pipeline.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (db.Database.IsSqlite())
    {
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }
    else
    {
        await db.Database.EnsureCreatedAsync();
    }

    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    var contextPath = Path.Combine(app.Environment.ContentRootPath, "../../.agents/skills/AGENT_CONTEXT.md");
    if (!File.Exists(contextPath))
    {
        contextPath = Path.Combine(Directory.GetCurrentDirectory(), ".agents/skills/AGENT_CONTEXT.md");
    }

    await seeder.SeedAsync(contextPath);
}

app.Run();
