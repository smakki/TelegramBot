using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBot.Models;
using TelegramBot.Options;

namespace TelegramBot
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHostedService<TelegramBotService>();
            builder.Services.AddHostedService<TelegramBotBackgroundService>();
            builder.Services.AddDbContext<TelegramBotDbContext>(options =>
                    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSQL")));
    
            builder.Services.AddTransient<ITelegramBotClient, TelegramBotClient>(serviceProvider =>
            {
                var token = serviceProvider.GetRequiredService<IOptions<TelegramOptions>>().Value.Token;
                return new TelegramBotClient(token);
            });
            builder.Services.AddSingleton<BotInteractionService>();
            builder.Services.AddSingleton<DatabaseServices>();
            builder.Services.AddSingleton<TelegramUtils>();
            builder.Services.AddSingleton<DateTimeUtils>();
            builder.Services.AddSingleton<RouteHandlers>();
            
            builder.Services.Configure<TelegramOptions>(builder.Configuration.GetSection(TelegramOptions.Telegram));
            
            var host = builder.Build();

            var startup = new Startup(host.Services);
            startup.Initialize(args);

            host.MapGet("/stats", (RouteHandlers handler, HttpContext context) =>handler.GetStatisticHandler(context));
            host.MapGet("/users", (RouteHandlers handler, HttpContext context) =>handler.GetUsersHandler(context));
            host.MapPost("/messages", (RouteHandlers handler, BroadcastMessage message) =>handler.PostMessageHandler(message));
 
            host.Run();
        }
    }
}