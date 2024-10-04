using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramBot.Bot.Services;
using TelegramBot.Bot.Services.MarkupService;
using TelegramBot.BotDbCOntext;
using TelegramBot.Categories.Entity;
using TelegramBot.Categories.Interfaces;
using TelegramBot.Categories.Services;
using TelegramBot.Products.Entity;
using TelegramBot.Products.Interfaces;
using TelegramBot.Products.Service;
using TelegramBot.Repository;
using TelegramBot.Services;
using TelegramBot.Users.Entity;
using TelegramBot.Users.Interfaces;
using TelegramBot.Users.Services;

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
builder.Services.AddScoped<IGenericRepository<Category> , GenericRepository<Category>>();
builder.Services.AddScoped<IGenericRepository<Product> , GenericRepository<Product>>();
builder.Services.AddScoped<IUserService, UserServices>();
builder.Services.AddScoped<IProductService , ProductService>();
builder.Services.AddScoped<ICateogryService, CategoryService>();

builder.Services.AddLocalization();

var app = builder.Build();

var supportedCultures = new[]{"uz-Uz" , "ru-Ru" , "en-Us"};
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
    app.UseRequestLocalization(localizationOptions);

app.Run();