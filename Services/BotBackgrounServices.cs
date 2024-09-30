
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace TelegramBot.Services;

public class BotBackgrounService : BackgroundService
{
    private readonly ILogger<BotBackgrounService> _logger;
    private readonly TelegramBotClient _client;
    private readonly IUpdateHandler _handler;

    public BotBackgrounService(
        ILogger<BotBackgrounService> logger,
        TelegramBotClient client,
        IUpdateHandler updateHandler)
        {
            _logger = logger;
            _client = client;
            _handler = updateHandler;
        }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var bot = await _client.GetMeAsync(stoppingToken);
        _logger.LogInformation("Bot started successfully: bot{bot.Username}", bot.Username);

        _client.StartReceiving(
            _handler.HandleUpdateAsync,
            _handler.HandlePollingErrorAsync,
            new ReceiverOptions()
            {
                ThrowPendingUpdates = true
            },
            stoppingToken   
        );

    }
}