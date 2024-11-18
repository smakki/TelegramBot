using TelegramBot.Options;

namespace TelegramBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<TelegramBotBackgroundService>();
            builder.Services.Configure<TelegramOptions>(builder.Configuration.GetSection(TelegramOptions.Telegram));

            var host = builder.Build();
            host.Run();
        }
    }
}