using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Bot.Services.MarkupService
{
    public interface IReplyMarkUpService
    {
        public Task<ReplyKeyboardMarkup> GenerateKeyboardMarkupForLanguageAsync();
        public Task<ReplyKeyboardMarkup> GenerateKeyboardMarkupForServicesAsync(string[] buttons);

    }
}
