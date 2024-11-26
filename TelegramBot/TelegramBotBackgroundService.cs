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
        private readonly DatabaseServices _dbService;
        private readonly BotInteractionService _botInteractionService;

        public TelegramBotBackgroundService(
            ITelegramBotClient botClient, 
            ILogger<TelegramBotBackgroundService> logger,
            IOptions<TelegramOptions> telegramOptions, 
            DatabaseServices dbService,
            BotInteractionService botInteractionService)
        {
            _logger = logger;
            _botClient = botClient;
            _options = telegramOptions.Value;
            _dbService = dbService;
            _botInteractionService = botInteractionService;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            token.Register(() =>
                _logger.LogInformation($"Background service is stopped"));

            while (!token.IsCancellationRequested)
            {
                var UpcomingReminders = _dbService.GetTasksForReminder(token);
                foreach (var task in UpcomingReminders)
                {
                    await _botInteractionService.SendReminder(task, token);
                    task.Remindered = true;
                    await _dbService.TaskUpdateAsync(task,token);
                }

                var UpcomingNotifications = _dbService.GetTasksForNotification(token);
                foreach (var task in UpcomingNotifications)
                {
                    await _botInteractionService.SendNotification(task, token);
                    task.Notificated = true;
                    await _dbService.TaskUpdateAsync(task, token);
                }

                try
                {
                    await Task.Delay(10000, token);
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
