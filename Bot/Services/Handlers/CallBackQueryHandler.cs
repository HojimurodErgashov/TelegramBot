using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Bot.Services.Handlers
{
    public class CallBackQueryHandler
    {
        public async Task HandleCallbackQueryAsync(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data == "Uzbek")
            {
            }
            else if (callbackQuery.Data == "Rus")
            {
            }
            // Boshqa tugmalarni qayta ishlash
        }
    }
}
