using Microsoft.AspNetCore.SignalR;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Hubs;

public class ChatHub : Hub
{
    public async Task JoinChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        await Clients.Group(chatId).SendAsync("UserJoined", Context.ConnectionId);
    }

    public async Task LeaveChat(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        await Clients.Group(chatId).SendAsync("UserLeft", Context.ConnectionId);
    }

    public async Task SendMessage(string chatId, string content)
    {
        var message = new CafePromenade.Core.Models.ChatMessage
        {
            ChatId = chatId,
            Content = content,
            SenderId = Context.ConnectionId,
            SenderName = "User",
            Timestamp = DateTime.UtcNow
        };
        await Clients.Group(chatId).SendAsync("ReceiveMessage", message);
    }

    public async Task SendTyping(string chatId)
    {
        await Clients.OthersInGroup(chatId).SendAsync("UserTyping", Context.ConnectionId);
    }
}
