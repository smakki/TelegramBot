using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBot.Options;

namespace TelegramBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHostedService<TelegramBotService>();
            builder.Services.AddHostedService<TelegramBotBackgroundService>();
            builder.Services.AddDbContext<TelegramBotDbContext>(options =>
                    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

            builder.Services.AddTransient<ITelegramBotClient, TelegramBotClient>(serviceProvider =>
            {
                var token = serviceProvider.GetRequiredService<IOptions<TelegramOptions>>().Value.Token;
                return new(token);
            });
            builder.Services.AddTransient<BotInteractionService>();
            builder.Services.Configure<TelegramOptions>(builder.Configuration.GetSection(TelegramOptions.Telegram));
            
            var host = builder.Build();
            
            host.Run();
        }
    }
}