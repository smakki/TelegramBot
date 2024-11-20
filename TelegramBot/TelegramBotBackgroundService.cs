using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBot.Options;

namespace TelegramBot
{
    public class TelegramBotBackgroundService : BackgroundService
    {
        private readonly ILogger<TelegramBotBackgroundService> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly TelegramOptions _options;

        public TelegramBotBackgroundService(ITelegramBotClient botClient, ILogger<TelegramBotBackgroundService> logger,IOptions<TelegramOptions> telegramOptions)
        {
            _logger = logger;
            _botClient = botClient;
            _options = telegramOptions.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() =>
                _logger.LogInformation($"Background service is stopped"));

            while (!stoppingToken.IsCancellationRequested)
            {
                //await _botClient.SendMessage(_options.AdminId, "test");

                try
                {
                    await Task.Delay(10000, stoppingToken);
                }
                catch (TaskCanceledException exception)
                {
                    _logger.LogCritical(exception, "TaskCanceledException Error", exception.Message);
                }
            }

            _logger.LogDebug($"Background service is stopped.");
        }
    }
}
