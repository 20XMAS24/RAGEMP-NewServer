using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Server.Application.Services;

namespace Server;

public class GameServer
{
    private readonly ILogger<GameServer> _logger;
    private readonly IConfiguration _configuration;
    private readonly IPlayerService _playerService;
    private readonly IVehicleService _vehicleService;
    private readonly IPropertyService _propertyService;

    public GameServer(
        ILogger<GameServer> logger,
        IConfiguration configuration,
        IPlayerService playerService,
        IVehicleService vehicleService,
        IPropertyService propertyService)
    {
        _logger = logger;
        _configuration = configuration;
        _playerService = playerService;
        _vehicleService = vehicleService;
        _propertyService = propertyService;
    }

    public async Task StartAsync()
    {
        var serverConfig = _configuration.GetSection("Server");
        var port = serverConfig.GetValue("Port", 22005);
        var maxPlayers = serverConfig.GetValue("MaxPlayers", 500);
        var serverName = serverConfig["Name"] ?? "RAGEMP Server";

        _logger.LogInformation("📡 Server Configuration:");
        _logger.LogInformation($"   Name: {serverName}");
        _logger.LogInformation($"   Port: {port}");
        _logger.LogInformation($"   Max Players: {maxPlayers}");
        _logger.LogInformation($"   Language: {serverConfig["Language"]}");
        _logger.LogInformation($"   Voice Chat: {serverConfig["VoiceChat"]}");

        _logger.LogInformation("✅ Server is ready! Waiting for RAGE:MP client connections...");
        
        // Keep server running
        await Task.Delay(Timeout.Infinite);
    }
}