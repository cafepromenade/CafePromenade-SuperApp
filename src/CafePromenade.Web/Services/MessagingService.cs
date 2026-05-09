using CafePromenade.Core.Models;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Services;

public class MessagingService : IMessagingService
{
    private readonly List<Chat> _chats = new();
    private readonly List<ChatMessage> _messages = new();

    public MessagingService()
    {
        _chats.Add(new Chat { Name = "General", Type = ChatType.Group });
        _chats.Add(new Chat { Name = "Development", Type = ChatType.Group });
        _chats.Add(new Chat { Name = "Telegram Bridge", Type = ChatType.Private });
    }

    public Task<Chat> CreateChatAsync(string name, ChatType type)
    {
        var chat = new Chat { Name = name, Type = type };
        _chats.Add(chat);
        return Task.FromResult(chat);
    }

    public Task<List<Chat>> GetChatsAsync() => Task.FromResult(_chats);
    public Task<Chat?> GetChatAsync(string chatId) => Task.FromResult(_chats.FirstOrDefault(c => c.Id == chatId));

    public Task<ChatMessage> SendMessageAsync(string chatId, string content, MessageType type = MessageType.Text)
    {
        var msg = new ChatMessage { ChatId = chatId, Content = content, Type = type, SenderName = "User" };
        _messages.Add(msg);
        var chat = _chats.FirstOrDefault(c => c.Id == chatId);
        if (chat != null) chat.LastMessage = msg;
        return Task.FromResult(msg);
    }

    public Task<List<ChatMessage>> GetMessagesAsync(string chatId, int limit = 50, int offset = 0)
    {
        return Task.FromResult(_messages.Where(m => m.ChatId == chatId).Skip(offset).Take(limit).ToList());
    }

    public Task<bool> DeleteMessageAsync(string messageId)
    {
        var msg = _messages.FirstOrDefault(m => m.Id == messageId);
        if (msg != null) { msg.IsDeleted = true; return Task.FromResult(true); }
        return Task.FromResult(false);
    }

    public Task<bool> EditMessageAsync(string messageId, string newContent)
    {
        var msg = _messages.FirstOrDefault(m => m.Id == messageId);
        if (msg != null) { msg.Content = newContent; msg.IsEdited = true; return Task.FromResult(true); }
        return Task.FromResult(false);
    }

    public Task<ChatParticipant> AddParticipantAsync(string chatId, string userId, string username)
    {
        var chat = _chats.FirstOrDefault(c => c.Id == chatId);
        var participant = new ChatParticipant { UserId = userId, Username = username, DisplayName = username };
        chat?.Participants.Add(participant);
        return Task.FromResult(participant);
    }

    public Task<bool> RemoveParticipantAsync(string chatId, string userId)
    {
        var chat = _chats.FirstOrDefault(c => c.Id == chatId);
        var p = chat?.Participants.FirstOrDefault(p => p.UserId == userId);
        if (p != null) { chat!.Participants.Remove(p); return Task.FromResult(true); }
        return Task.FromResult(false);
    }

    public Task<List<ChatParticipant>> GetParticipantsAsync(string chatId)
    {
        return Task.FromResult(_chats.FirstOrDefault(c => c.Id == chatId)?.Participants ?? new List<ChatParticipant>());
    }

    public Task<bool> MarkAsReadAsync(string chatId, string messageId)
    {
        var msg = _messages.FirstOrDefault(m => m.Id == messageId);
        if (msg != null) { msg.IsRead = true; return Task.FromResult(true); }
        return Task.FromResult(false);
    }

    public Task<ChatMessage?> GetMessageAsync(string messageId)
    {
        return Task.FromResult(_messages.FirstOrDefault(m => m.Id == messageId));
    }

    public Task<List<ChatMessage>> SearchMessagesAsync(string chatId, string query)
    {
        return Task.FromResult(_messages.Where(m => m.ChatId == chatId && m.Content.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList());
    }
}
