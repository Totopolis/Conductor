using Microsoft.Extensions.Options;
using Server.Abstractions;

namespace Server.Boot.MaintenanceBot;

internal sealed class MaintenanceBotSender : IMaintenanceBot, IDisposable
{
    private MaintenanceBotSettings _settings;
    private readonly string _urlTemplate;
    private readonly HttpClient _httpClient;

    // TODO: use observable settings
    public MaintenanceBotSender(
        IOptions<MaintenanceBotSettings> settings,
        // TODO: use factory
        HttpClient httpClient)
    {
        _settings = settings.Value;

        _urlTemplate = "https://api.telegram.org/bot{API}/sendMessage?chat_id={CHATID}&text={MESSAGE}"
            .Replace("{API}", settings.Value.ApiKey)
            .Replace("{CHATID}", settings.Value.ChatId);

        _httpClient = httpClient;
    }

    public async Task BroadcastMessage(string message)
    {
        if (!_settings.Enable)
        {
            return;
        }

        message = Uri.EscapeDataString(message);
        var uri = _urlTemplate.Replace("{MESSAGE}", message);
        await _httpClient.SendAsync(new HttpRequestMessage()
        {
            RequestUri = new Uri(uri)
        });
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
