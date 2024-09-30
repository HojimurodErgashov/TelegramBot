using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Services;

public partial  class BotUpdateHandler
{
    private async Task HandleEditMessageAsync(ITelegramBotClient client, Message message , CancellationToken token)
    {
        var from = message.From;
        _logger.LogInformation("Received  Edit Message from {from.FirstName}: {Message.Text}", from?.FirstName, message.Text);
    }
}