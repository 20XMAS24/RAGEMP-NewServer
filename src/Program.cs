using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Server.Application.Services;
using Server.Infrastructure.Data;
using Server.Infrastructure.Persistence;
using Server.Infrastructure.Security;

namespace Server;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();
        ConfigureServices(services, configuration);
        var serviceProvider = services.BuildServiceProvider();

        using var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            logger.LogInformation("🚀 Starting RAGE:MP Server...");
            await InitializeDatabase(serviceProvider, logger);
            await SeedDefaultData(serviceProvider, logger);
            
            var gameServer = serviceProvider.GetRequiredService<GameServer>();
            await gameServer.StartAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Server startup failed");
            throw;
        }
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.AddSingleton(configuration);

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Database
        var connectionString = BuildConnectionString(configuration);
        services.AddScoped(_ => new AppDbContext(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Security
        services.AddSingleton<IPasswordHasher>(new PasswordHasher(
            workFactor: configuration.GetValue("Security:PasswordHashIterations", 12)
        ));

        // Services
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IVehicleService, VehicleService>();n        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IBankService, BankService>();

        // Game Server
        services.AddSingleton<GameServer>();
    }

    private static async Task InitializeDatabase(IServiceProvider serviceProvider, ILogger<Program> logger)
    {
        logger.LogInformation("🔧 Initializing database...");
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        
        try
        {
            await dbContext.Database.EnsureCreatedAsync();
            logger.LogInformation("✅ Database initialized successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Database initialization failed");
            throw;
        }
    }

    private static async Task SeedDefaultData(IServiceProvider serviceProvider, ILogger<Program> logger)
    {
        logger.LogInformation("🌱 Seeding default data...");
        var jobService = serviceProvider.GetRequiredService<IJobService>();
        
        try
        {
            // Add default jobs if they don't exist
            var jobs = new[] 
            { 
                new { Name = "Taxi Driver", Salary = 5000L, Level = 1 },
                new { Name = "Truck Driver", Salary = 8000L, Level = 5 },
                new { Name = "Mechanic", Salary = 10000L, Level = 10 },
                new { Name = "Police Officer", Salary = 12000L, Level = 15 },
                new { Name = "Doctor", Salary = 15000L, Level = 20 },
            };

            foreach (var job in jobs)
            {
                await jobService.CreateJobIfNotExistsAsync(job.Name, job.Salary, job.Level);
            }

            logger.LogInformation("✅ Default data seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "⚠️ Seeding default data encountered an issue");
        }
    }

    private static string BuildConnectionString(IConfiguration configuration)
    {
        var db = configuration.GetSection("Database");
        var server = db["Host"] ?? "localhost";
        var port = db.GetValue("Port", 3306);
        var user = db["User"] ?? "root";
        var password = db["Password"];
        var database = db["Database"] ?? "ragemp_db";

        return $"Server={server};Port={port};User Id={user};Password={password};Database={database};";
    }
}