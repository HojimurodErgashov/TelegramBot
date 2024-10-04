using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Bot.Services.MarkupService
{
    public interface IReplyMarkUpService
    {
        public ReplyKeyboardMarkup GenerateKeyboardMarkupForLanguage();
        public ReplyKeyboardMarkup GenerateKeyboardMarkupForServices(string[] buttons);
        public ReplyKeyboardMarkup GenerateMarkupForCategories(string[] categories);
    }
}