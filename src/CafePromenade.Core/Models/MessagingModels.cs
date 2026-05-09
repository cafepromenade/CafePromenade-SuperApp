namespace CafePromenade.Core.Models;

public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SenderId { get; set; } = "";
    public string SenderName { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public MessageType Type { get; set; } = MessageType.Text;
    public string? AttachmentUrl { get; set; }
    public string ChatId { get; set; } = "";
    public bool IsRead { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    public string? ReplyToId { get; set; }
}

public class Chat
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public ChatType Type { get; set; } = ChatType.Private;
    public List<ChatParticipant> Participants { get; set; } = new();
    public ChatMessage? LastMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? AvatarColor { get; set; }
    public int UnreadCount { get; set; }
}

public class ChatParticipant
{
    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
    public string? AvatarUrl { get; set; }
    public ParticipantRole Role { get; set; } = ParticipantRole.Member;
}

public class TelegramConfig
{
    public string BotToken { get; set; } = "";
    public string ChatId { get; set; } = "";
    public bool IsConnected { get; set; }
    public string WebhookUrl { get; set; } = "";
    public List<TelegramUpdate> RecentUpdates { get; set; } = new();
}

public class TelegramUpdate
{
    public long UpdateId { get; set; }
    public string? MessageText { get; set; }
    public string? FromUsername { get; set; }
    public long ChatId { get; set; }
    public DateTime Timestamp { get; set; }
}

public class MessagingServerConfig
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5180;
    public bool UseHttps { get; set; }
    public int MaxConnections { get; set; } = 100;
    public bool EnableTelegramBridge { get; set; }
    public string ServerName { get; set; } = "CafePromenade Messenger";
}

public enum MessageType
{
    Text,
    Image,
    File,
    Voice,
    Video,
    Location,
    Sticker,
    System
}

public enum ChatType
{
    Private,
    Group,
    Channel,
    Supergroup
}

public enum ParticipantRole
{
    Owner,
    Admin,
    Member
}
