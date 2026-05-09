using Microsoft.AspNetCore.Mvc;
using CafePromenade.Core.Models;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagingController : ControllerBase
{
    private readonly IMessagingService _messaging;
    private readonly ITelegramService _telegram;

    public MessagingController(IMessagingService messaging, ITelegramService telegram)
    {
        _messaging = messaging;
        _telegram = telegram;
    }

    [HttpGet("chats")]
    public async Task<IActionResult> GetChats() => Ok(await _messaging.GetChatsAsync());

    [HttpPost("chats")]
    public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request)
    {
        var chat = await _messaging.CreateChatAsync(request.Name, request.Type);
        return Ok(chat);
    }

    [HttpGet("chats/{chatId}")]
    public async Task<IActionResult> GetChat(string chatId)
    {
        var chat = await _messaging.GetChatAsync(chatId);
        return chat == null ? NotFound() : Ok(chat);
    }

    [HttpGet("chats/{chatId}/messages")]
    public async Task<IActionResult> GetMessages(string chatId, [FromQuery] int limit = 50, [FromQuery] int offset = 0)
    {
        return Ok(await _messaging.GetMessagesAsync(chatId, limit, offset));
    }

    [HttpPost("chats/{chatId}/messages")]
    public async Task<IActionResult> SendMessage(string chatId, [FromBody] SendMessageRequest request)
    {
        var msg = await _messaging.SendMessageAsync(chatId, request.Content, request.Type);
        return Ok(msg);
    }

    [HttpDelete("messages/{messageId}")]
    public async Task<IActionResult> DeleteMessage(string messageId)
    {
        var result = await _messaging.DeleteMessageAsync(messageId);
        return result ? Ok() : NotFound();
    }

    [HttpPut("messages/{messageId}")]
    public async Task<IActionResult> EditMessage(string messageId, [FromBody] EditMessageRequest request)
    {
        var result = await _messaging.EditMessageAsync(messageId, request.Content);
        return result ? Ok() : NotFound();
    }

    [HttpGet("chats/{chatId}/participants")]
    public async Task<IActionResult> GetParticipants(string chatId) => Ok(await _messaging.GetParticipantsAsync(chatId));

    [HttpPost("chats/{chatId}/participants")]
    public async Task<IActionResult> AddParticipant(string chatId, [FromBody] AddParticipantRequest request)
    {
        var participant = await _messaging.AddParticipantAsync(chatId, request.UserId, request.Username);
        return Ok(participant);
    }

    [HttpDelete("chats/{chatId}/participants/{userId}")]
    public async Task<IActionResult> RemoveParticipant(string chatId, string userId)
    {
        var result = await _messaging.RemoveParticipantAsync(chatId, userId);
        return result ? Ok() : NotFound();
    }

    [HttpGet("chats/{chatId}/search")]
    public async Task<IActionResult> Search(string chatId, [FromQuery] string query)
    {
        return Ok(await _messaging.SearchMessagesAsync(chatId, query));
    }

    [HttpPost("telegram/connect")]
    public async Task<IActionResult> ConnectTelegram([FromBody] ConnectTelegramRequest request)
    {
        var result = await _telegram.ConnectAsync(request.BotToken);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("telegram/disconnect")]
    public async Task<IActionResult> DisconnectTelegram()
    {
        await _telegram.DisconnectAsync();
        return Ok();
    }

    [HttpPost("telegram/send")]
    public async Task<IActionResult> SendTelegram([FromBody] SendTelegramRequest request)
    {
        var result = await _telegram.SendMessageAsync(request.ChatId, request.Text);
        return result ? Ok() : StatusCode(500);
    }

    [HttpGet("telegram/updates")]
    public async Task<IActionResult> GetTelegramUpdates() => Ok(await _telegram.GetUpdatesAsync());

    [HttpGet("telegram/status")]
    public async Task<IActionResult> TelegramStatus() => Ok(new { connected = await _telegram.IsConnectedAsync() });
}

public class CreateChatRequest { public string Name { get; set; } = ""; public ChatType Type { get; set; } }
public class SendMessageRequest { public string Content { get; set; } = ""; public MessageType Type { get; set; } = MessageType.Text; }
public class EditMessageRequest { public string Content { get; set; } = ""; }
public class AddParticipantRequest { public string UserId { get; set; } = ""; public string Username { get; set; } = ""; }
public class ConnectTelegramRequest { public string BotToken { get; set; } = ""; }
public class SendTelegramRequest { public string ChatId { get; set; } = ""; public string Text { get; set; } = ""; }
