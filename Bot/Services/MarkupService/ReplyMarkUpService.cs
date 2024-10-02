using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Bot.Services.MarkupService
{
    public class ReplyMarkUpService : IReplyMarkUpService
    {
        public async Task<ReplyKeyboardMarkup> GenerateKeyboardMarkupForLanguageAsync()
        {
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
            {
            new[]
            {
                new KeyboardButton("O'zbek"),
                new KeyboardButton("English")
            }
       })
            {
                ResizeKeyboard = true
            };
            return replyKeyboardMarkup;
        }

        public async Task<ReplyKeyboardMarkup> GenerateKeyboardMarkupForServicesAsync(string[] buttons)
        {
            var keyboardButtons = buttons.Select(button => new KeyboardButton(button)).ToArray();

            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                keyboardButtons.Take(1).ToArray(),
                buttons.Skip(1).Take(2).Select(button => new KeyboardButton(button)).ToArray(),
                buttons.Skip(3).Take(2).Select(button => new KeyboardButton(button)).ToArray()
            })
            {
                ResizeKeyboard = true // Tugmachalarni o'lchamlarini o'zgartirish
            };
            return keyboard;
        }
    }
}
