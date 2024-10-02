using TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramBot.BotDbCOntext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TelegramBot.Bot.Services.MarkupService;
using TelegramBot.Bot.Services;
using TelegramBot.Users.Interfaces;
using TelegramBot.Users.Services;
using TelegramBot.Repository;
using TelegramBot.Users.Entity;

var builder = WebApplication.CreateBuilder(args);
var token = builder.Configuration.GetValue("BotToken" , string.Empty);
var connectionString = builder.Configuration.GetConnectionString("BotDatabase");

builder.Services.AddDbContext<BotDbContext>(options =>
    {
        options.UseNpgsql(connectionString);
        options.EnableSensitiveDataLogging();
    });

builder.Services.AddSingleton(new TelegramBotClient(token));
builder.Services.AddSingleton<IUpdateHandler , BotUpdateHandler>();
builder.Services.AddHostedService<BotBackgrounService>();
builder.Services.AddScoped<IReplyMarkUpService, ReplyMarkUpService>();
builder.Services.AddScoped<IGenericRepository<User> , GenericRepository<User>>();
builder.Services.AddScoped<IUserService, UserServices>();

builder.Services.AddLocalization();

var app = builder.Build();

var supportedCultures = new[]{"uz-Uz" , "ru-Ru" , "en-Us"};
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
    app.UseRequestLocalization(localizationOptions);

app.Run();