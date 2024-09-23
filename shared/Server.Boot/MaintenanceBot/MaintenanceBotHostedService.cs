using Server.Abstractions;

namespace Server.Boot.MaintenanceBot;

internal sealed class MaintenanceBotHostedService : BackgroundService
{
    private readonly IMaintenanceBot _botSender;
    private readonly ISystemInfo _systemInfo;
    private readonly ILogger<MaintenanceBotHostedService> _logger;

    public MaintenanceBotHostedService(
        IMaintenanceBot botSender,
        ISystemInfo systemInfo,
        ILogger<MaintenanceBotHostedService> logger)
    {
        _botSender = botSender;
        _systemInfo = systemInfo;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!_systemInfo.IsDevelopment)
            {
                await _botSender.BroadcastMessage(@$"Starship APPNAME flyout.
Version: {ThisAssembly.Info.InformationalVersion}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MaintenanceBot broadcast message not work");
        }

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000);
        }
    }
}
