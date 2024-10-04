using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Bot.Services.MarkupService
{
    public class ReplyMarkUpService : IReplyMarkUpService
    {
        public ReplyKeyboardMarkup GenerateKeyboardMarkupForLanguage()
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

        public ReplyKeyboardMarkup GenerateKeyboardMarkupForServices(string[] buttons)
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

        public ReplyKeyboardMarkup GenerateMarkupForCategories(string[] categories) 
        {
            var keyboard = new List<List<KeyboardButton>>();
            keyboard.Add(new List<KeyboardButton> { new KeyboardButton(categories[0]) });
            var remainingWords = categories.Skip(1).ToArray();

            for (int i = 0; i < remainingWords.Length; i += 2)
            {
                var row = new List<KeyboardButton>
                {
                    new KeyboardButton(remainingWords[i])
                };

                if (i + 1 < remainingWords.Length)
                {
                    row.Add(new KeyboardButton(remainingWords[i + 1]));
                }

                keyboard.Add(row);
            }

            return new ReplyKeyboardMarkup(keyboard)
            {
                ResizeKeyboard = true
            };
        }
    }
}