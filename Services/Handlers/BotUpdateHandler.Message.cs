using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Services;
public partial class BotUpdateHandler
{
    private async Task HandleMessageAsync(ITelegramBotClient client , Message? message , CancellationToken token)
        {

            var from = message.From;
        _logger.LogInformation("Received message from {from.FirstName}", from?.FirstName);
    }
}