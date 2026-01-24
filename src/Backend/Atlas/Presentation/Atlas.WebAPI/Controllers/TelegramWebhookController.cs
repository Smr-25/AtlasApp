using System.Text.Json;
using Atlas.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelegramWebhookController(IAccountService accountService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();

        if (string.IsNullOrEmpty(body))
            return Ok();

        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;

        if (!root.TryGetProperty("message", out var message))
            return Ok();

        if (!message.TryGetProperty("text", out var textElement))
            return Ok();

        var messageText = textElement.GetString();
        if (string.IsNullOrEmpty(messageText))
            return Ok();

        if (!message.TryGetProperty("chat", out var chat))
            return Ok();

        if (!chat.TryGetProperty("id", out var chatIdElement))
            return Ok();

        var chatId = chatIdElement.GetInt64().ToString();

        if (messageText.StartsWith("/start "))
        {
            var linkCode = messageText.Replace("/start ", "").Trim();

            if (!string.IsNullOrEmpty(linkCode))
            {
                await accountService.LinkTelegramByChatIdAsync(linkCode, chatId);
            }
        }

        return Ok();
    }
}