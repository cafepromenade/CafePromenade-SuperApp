using CafePromenade.Core.Models;
using CafePromenade.Core.Services;
using System.Text;
using System.Text.Json;

namespace CafePromenade.Web.Services;

public class TelegramService : ITelegramService
{
    private readonly HttpClient _http = new();
    private string _botToken = "";
    private bool _connected;

    public async Task<bool> ConnectAsync(string botToken)
    {
        _botToken = botToken;
        try
        {
            var response = await _http.GetStringAsync($"https://api.telegram.org/bot{botToken}/getMe");
            _connected = response.Contains("\"ok\":true");
            return _connected;
        }
        catch { return false; }
    }

    public Task<bool> DisconnectAsync()
    {
        _connected = false;
        _botToken = "";
        return Task.FromResult(true);
    }

    public async Task<bool> SendMessageAsync(string chatId, string text)
    {
        if (!_connected) return false;
        var payload = JsonSerializer.Serialize(new { chat_id = chatId, text });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _http.PostAsync($"https://api.telegram.org/bot{_botToken}/sendMessage", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<TelegramUpdate>> GetUpdatesAsync()
    {
        if (!_connected) return new List<TelegramUpdate>();
        try
        {
            var response = await _http.GetStringAsync($"https://api.telegram.org/bot{_botToken}/getUpdates");
            var doc = JsonDocument.Parse(response);
            var updates = new List<TelegramUpdate>();
            if (doc.RootElement.TryGetProperty("result", out var result))
            {
                foreach (var item in result.EnumerateArray())
                {
                    updates.Add(new TelegramUpdate
                    {
                        UpdateId = item.GetProperty("update_id").GetInt64(),
                        MessageText = item.TryGetProperty("message", out var msg) && msg.TryGetProperty("text", out var text) ? text.GetString() : null,
                        FromUsername = item.TryGetProperty("message", out var m) && m.TryGetProperty("from", out var from) && from.TryGetProperty("username", out var u) ? u.GetString() : null,
                        Timestamp = DateTime.UtcNow
                    });
                }
            }
            return updates;
        }
        catch { return new List<TelegramUpdate>(); }
    }

    public async Task<bool> SetWebhookAsync(string url)
    {
        var payload = JsonSerializer.Serialize(new { url });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _http.PostAsync($"https://api.telegram.org/bot{_botToken}/setWebhook", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteWebhookAsync()
    {
        var response = await _http.GetAsync($"https://api.telegram.org/bot{_botToken}/deleteWebhook");
        return response.IsSuccessStatusCode;
    }

    public Task<TelegramConfig> GetConfigAsync()
    {
        return Task.FromResult(new TelegramConfig { BotToken = _botToken, IsConnected = _connected });
    }

    public Task<bool> IsConnectedAsync() => Task.FromResult(_connected);

    public async Task<string> GetBotInfoAsync()
    {
        if (!_connected) return "Not connected";
        try { return await _http.GetStringAsync($"https://api.telegram.org/bot{_botToken}/getMe"); }
        catch { return "Error"; }
    }
}
