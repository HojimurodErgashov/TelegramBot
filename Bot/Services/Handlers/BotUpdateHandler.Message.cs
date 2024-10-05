using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Bot.Entity;
using TelegramBot.Categories.Entity;
using TelegramBot.Products.Entity;

namespace TelegramBot.Services;
public partial class BotUpdateHandler
{
    const string phoneNumberPattern = @"^\+998\d{9}$";
    string fullNamepattern = @"^[a-zA-Z\s]+$";
    private Dictionary<long, Users.Entity.User> userStates = new Dictionary<long, Users.Entity.User>();
    public List<Category> categories = new List<Category>();
    private async Task HandleMessageAsync(ITelegramBotClient client , Message? message , CancellationToken token)
    {
         var from = message.From;
        _logger.LogInformation("Received message from {from.FirstName}", from?.FirstName);

        var handler = message.Type switch
        {
            MessageType.Text => HandleTextMessageAsync(client, message, token),
            _ => HandleUnknownMessageAsync(client, message, token)
        };
        await handler;
    }

    private async Task HandleUnknownMessageAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {
        _logger.LogInformation($"Received message type {message.Type}");
    }

    private async Task HandleTextMessageAsync(ITelegramBotClient client, Message? message, CancellationToken token)
    {
        var chatId = message.Chat.Id;
        var language = new string[] { "O'zbek", "English" };

        if (!userStates.ContainsKey(chatId))
        {
            userStates[chatId] = new Users.Entity.User { CurrentState = BotState.Start  , LanguageCode = "uz-Uz"};
        }

        var user = userStates[chatId];
        user.Id = chatId;
        var userEntity = await _userService.GetUserAsync(chatId);

            if (message.Text == "/start")
            {
                user.CurrentState = BotState.Start;
            }


            CultureInfo? culture = new CultureInfo(user.LanguageCode);

            switch (user.CurrentState)
            {
                case BotState.Start:
                    SetCulture(user.LanguageCode, ref culture);
                    user.CurrentState = BotState.FullName;
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: _localizer["choose-language"],
                        replyToMessageId: message.MessageId,
                        replyMarkup: _replyMarkUpService.GenerateKeyboardMarkupForLanguage(),
                        cancellationToken: token);
                    break;

                case BotState.FullName:

                    if (language.Contains(message.Text))
                    {
                        user.LanguageCode = message.Text == "O'zbek" ? "uz-Uz" : "en-Us";
                        SetCulture(user.LanguageCode, ref culture);
                        user.CurrentState = BotState.PhoneNumber;

                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: _localizer["GetName"],
                            replyToMessageId: message.MessageId,
                            replyMarkup: null,
                            cancellationToken: token);
                    }
                    else
                    {
                        user.CurrentState = BotState.FullName;

                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: _localizer["choose-language"],
                            replyToMessageId: message.MessageId,
                            replyMarkup: null,
                            cancellationToken: token);
                    }
                    break;
                case BotState.PhoneNumber:
                    SetCulture(user.LanguageCode, ref culture);
                    if (Regex.IsMatch(message.Text, fullNamepattern))
                    {
                        user.FullName = message.Text;
                        user.CurrentState = BotState.Code;

                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: _localizer["GetPhoneNumber"],
                            replyToMessageId: message.MessageId,
                            replyMarkup: null,
                            cancellationToken: token);
                    }
                    else
                    {
                        user.LanguageCode = message.Text == "O'zbek" ? "uz-Uz" : "en-Us";
                        SetCulture(user.LanguageCode, ref culture);
                        user.CurrentState = BotState.PhoneNumber;

                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: _localizer["GetName"],
                            replyToMessageId: message.MessageId,
                            replyMarkup: null,
                            cancellationToken: token);
                    }
                    break;
                case BotState.Code:
                    if (Regex.IsMatch(message.Text, phoneNumberPattern))
                    {

                        SetCulture(user.LanguageCode, ref culture);
                        user.PhoneNumber = message.Text;
                        user.CurrentState = BotState.Service;
                        user.Code = new Random().Next(1000, 10000);

                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: _localizer["ConfirmCode"] + user.Code,
                            replyToMessageId: message.MessageId,
                            replyMarkup: null,
                            cancellationToken: token);
                    }
                    else
                    {
                        user.CurrentState = BotState.Code;

                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: _localizer["GetPhoneNumber"],
                            replyToMessageId: message.MessageId,
                            replyMarkup: null,
                            cancellationToken: token);
                    }
                    break;

                case BotState.Service:
                    var arr = _localizer["Service"].ToString().Split('!');
                    SetCulture(user.LanguageCode, ref culture);
                    if (message.Text == user.Code.ToString())
                    {
                        user.CurrentState = BotState.Choose;
                        user.Code = Convert.ToInt32(message.Text);
                        var response = await _userService.GetUserAsync(message.From.Id);
                        var str = "";
                        if (response == null)
                        {
                            response = await _userService.CreateUserAsync(user);
                            str = _localizer["Services"];
                            //// response null bo'lishini ko'rib qo'yish kerak =>  ef core exception holati;
                        }
                        else
                        {
                            str = _localizer["AlreadyRegistered"];
                        }

                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: str,
                            replyToMessageId: message.MessageId,
                            replyMarkup: _replyMarkUpService.GenerateKeyboardMarkupForServices(arr),
                            cancellationToken: token);
                        break;
                    }
                    else
                    {
                        SetCulture(user.LanguageCode, ref culture);
                        user.CurrentState = BotState.Service;
                        user.Code = new Random().Next(1000, 10000);

                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: _localizer["ConfirmCode"],
                            replyToMessageId: message.MessageId,
                            replyMarkup: null,
                            cancellationToken: token);
                        break;
                    }
                case BotState.Choose:
                    SetCulture(user.LanguageCode, ref culture);
                    if (message.Text.Equals("Placing an order") || message.Text.Equals("Buyurtma berish"))
                    {
                        user.CurrentState = BotState.Category;

                        categories = (await _categoryService.GetAllCategoryAsync(x => true, new string[] { "Products" }, true)).ToList();

                        string[] strings = new string[categories.Count + 1];
                        strings[0] = _localizer["Orqaga"].ToString();
                        var i = 1;
                        foreach (var category in categories)
                        {
                            strings[i++] = user.LanguageCode.Equals("en-Us") ? category.Name_en : category.Name_uz;
                        }

                        await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: _localizer["categories"],
                        replyToMessageId: message.MessageId,
                        replyMarkup: _replyMarkUpService.GenerateMarkupForCategories(strings),
                        cancellationToken: token);

                    }/*else if(message.Text.Equals(""))*/
                    break;
                case BotState.Category:
                ///// category tanlash oynasidan ortga qaytish
                if (message.Text.Equals("Orqaga"))
                {
                    arr = _localizer["Service"].ToString().Split('!');
                    user.CurrentState = BotState.Choose;
                    await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: _localizer["Services"],
                            replyToMessageId: message.MessageId,
                            replyMarkup: _replyMarkUpService.GenerateKeyboardMarkupForServices(arr),
                            cancellationToken: token);
                    break;
                }
                ////Category oynasidan category tanlashga javon  Category name keladi o'sha name bo'yicha bizga o'sha categorydagi productlar qaytib keladi.
                else
                {
                    List<Product>? products = new List<Product>();
                    Category  category = categories.FirstOrDefault(c => IsUzbek(user) ?c.Name_uz.Equals(message.Text) : c.Name_en.Equals(message.Text));
                    products = category.Products;
                    arr = new string[products.Count + 1];
                    arr[0] = IsUzbek(userEntity) ? "Orqaga" : "Back";
                    long i = 1;
                    foreach (Product product in products) 
                    {
                        arr[i++] = IsUzbek(user) ? product.Name_uz : product.Name_en;
                    }

                    user.CurrentState = BotState.Products;
                    await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Product tanlang:",
                            replyToMessageId: message.MessageId,
                            replyMarkup: _replyMarkUpService.GenerateKeyboardMarkupForServices(arr),
                            cancellationToken: token);
                    break;

                }
                
                break;

            }


            /*        await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text:_localizer["choose-language"],
                        replyToMessageId: message.MessageId,
                        cancellationToken:token);*/
    }

    private void SetCulture(string LanguageCode , ref CultureInfo culture)
    {
        culture = new CultureInfo(LanguageCode);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    private bool IsUzbek(TelegramBot.Users.Entity.User user)
    {
        if (user.LanguageCode == "uz-Uz")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}