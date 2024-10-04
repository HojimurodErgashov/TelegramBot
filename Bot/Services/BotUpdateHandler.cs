using Microsoft.Extensions.Localization;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Bot.Resources;
using TelegramBot.Bot.Services.MarkupService;
using TelegramBot.Categories.Interfaces;
using TelegramBot.Users.Interfaces;

namespace TelegramBot.Services;

public partial class BotUpdateHandler : IUpdateHandler
{
    private readonly ILogger<BotUpdateHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private  IStringLocalizer _localizer;
    private  IReplyMarkUpService _replyMarkUpService;
    private  IProductService _userService;
    private ICateogryService _categoryService;
    private IProductService _productService;

    public BotUpdateHandler(ILogger<BotUpdateHandler> logger ,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Error occured wiht Telegram Bot: {e.Message}", exception);
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {

        using var scope = _scopeFactory.CreateScope();
        _userService = scope.ServiceProvider.GetRequiredService<IProductService>();
        _categoryService = scope.ServiceProvider.GetRequiredService<ICateogryService>();
        _productService = scope.ServiceProvider.GetRequiredService<IProductService>();
        _replyMarkUpService = scope.ServiceProvider.GetRequiredService<IReplyMarkUpService>();



        var culture = await GetCultureForUser(update);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        _localizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<BotLocalizer>>();



        var handler = update.Type switch
        {
            UpdateType.Message => HandleMessageAsync(botClient, update.Message, cancellationToken),
            UpdateType.EditedMessage => HandleEditMessageAsync(botClient, update.EditedMessage, cancellationToken),
            _ => HandleUnknownMessage(botClient, update, cancellationToken)
        };

        try
        {
            await handler;
        }
        catch (Exception ex)
        {
            await HandlePollingErrorAsync(botClient, ex, cancellationToken);
        }

    }

    public async Task<CultureInfo> GetCultureForUser(Update update)
    {
        User? from = update.Type switch
        {
            UpdateType.Message => update.Message.From,
            UpdateType.EditedChannelPost => update.EditedMessage.From,
            UpdateType.CallbackQuery => update.CallbackQuery.From,
            UpdateType.InlineQuery => update.InlineQuery.From,
            _ => update?.Message?.From
        };

        var user = await _userService.GetUserAsync(from?.Id);
        return new CultureInfo(user?.LanguageCode ?? "uz-Uz");
;//
    }

    private Task HandleUnknownMessage(ITelegramBotClient client, Update update, CancellationToken token)
    {
        _logger.LogInformation("Update Type {update.Type} received", update.Type);
        return Task.CompletedTask;
    }

}