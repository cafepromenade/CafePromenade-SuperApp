using CafePromenade.Core.Models;

namespace CafePromenade.Core.Services;

public interface IMessagingService
{
    Task<Chat> CreateChatAsync(string name, ChatType type);
    Task<List<Chat>> GetChatsAsync();
    Task<Chat?> GetChatAsync(string chatId);
    Task<ChatMessage> SendMessageAsync(string chatId, string content, MessageType type = MessageType.Text);
    Task<List<ChatMessage>> GetMessagesAsync(string chatId, int limit = 50, int offset = 0);
    Task<bool> DeleteMessageAsync(string messageId);
    Task<bool> EditMessageAsync(string messageId, string newContent);
    Task<ChatParticipant> AddParticipantAsync(string chatId, string userId, string username);
    Task<bool> RemoveParticipantAsync(string chatId, string userId);
    Task<List<ChatParticipant>> GetParticipantsAsync(string chatId);
    Task<bool> MarkAsReadAsync(string chatId, string messageId);
    Task<ChatMessage?> GetMessageAsync(string messageId);
    Task<List<ChatMessage>> SearchMessagesAsync(string chatId, string query);
}

public interface ITelegramService
{
    Task<bool> ConnectAsync(string botToken);
    Task<bool> DisconnectAsync();
    Task<bool> SendMessageAsync(string chatId, string text);
    Task<List<TelegramUpdate>> GetUpdatesAsync();
    Task<bool> SetWebhookAsync(string url);
    Task<bool> DeleteWebhookAsync();
    Task<TelegramConfig> GetConfigAsync();
    Task<bool> IsConnectedAsync();
    Task<string> GetBotInfoAsync();
}
