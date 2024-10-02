using System.Globalization;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Bot.Entity;

namespace TelegramBot.Services;
public partial class BotUpdateHandler
{
    const string phoneNumberPattern = @"^\+998\d{9}$";
    string fullNamepattern = @"^[a-zA-Z\s]+$";
    private Dictionary<long, Users.Entity.User> userStates = new Dictionary<long, Users.Entity.User>();
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
                    replyMarkup: await _replyMarkUpService.GenerateKeyboardMarkupForLanguageAsync(),
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
                if (Regex.IsMatch(message.Text , fullNamepattern))
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
                if (Regex.IsMatch(message.Text , phoneNumberPattern))
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
                if(message.Text == user.Code.ToString())
                {
                    bool isHave = false;
                    user.Code = Convert.ToInt32(message.Text);

                    var response = await _userService.GetUserAsync(message.From.Id);

                    if (response == null)
                    {
                        isHave = true;
                        response = await _userService.CreateUserAsync(user);
                    }

                    if (response != null && isHave)
                    {
                        var arr = new string[] { "Buyurtma berish", "Mening buyurtmalarim", "Izoh qoldirish", "Biz haqimizda", "Sozlamalar" };
                        SetCulture(user.LanguageCode, ref culture);

                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: _localizer["Services"],
                            replyToMessageId: message.MessageId,
                            replyMarkup: await _replyMarkUpService.GenerateKeyboardMarkupForServicesAsync(arr),
                            cancellationToken: token);
                    }
                    else
                    {   
                        user.CurrentState = BotState.Start;
                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: _localizer["BadResponse"] + user.Code,
                            replyToMessageId: message.MessageId,
                            replyMarkup: null,
                            cancellationToken: token);
                    }
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
}