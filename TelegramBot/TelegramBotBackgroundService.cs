namespace TelegramBot
{
    public class TelegramBotBackgroundService(
        ILogger<TelegramBotBackgroundService> logger,
        DatabaseServices dbService,
        BotInteractionService botInteractionService)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken token)
        {
            token.Register(() =>
                logger.LogInformation($"Background service is stopped"));

            while (!token.IsCancellationRequested)
            {
                var upcomingReminders = dbService.GetTasksForReminder(token);
                foreach (var task in upcomingReminders)
                {
                    await botInteractionService.SendReminder(task, token);
                    task.Reminded = true;
                    await dbService.TaskUpdateAsync(task,token);
                }

                var upcomingNotifications = dbService.GetTasksForNotification(token);
                foreach (var task in upcomingNotifications)
                {
                    await botInteractionService.SendNotification(task, token);
                    task.Notified = true;
                    await dbService.TaskUpdateAsync(task, token);
                }

                try
                {
                    await Task.Delay(10000, token);
                }
                catch (TaskCanceledException exception)
                {
                    logger.LogCritical(exception, "TaskCanceledException Error\n\r"+ exception.Message);
                }
            }

            logger.LogDebug($"Background service is stopped.");
        }
    }
}
