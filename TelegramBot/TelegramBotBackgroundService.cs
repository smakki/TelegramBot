using Telegram.Bot.Requests;
using Telegram.Bot.Types.Enums;
using TelegramBot.Models;

namespace TelegramBot
{
    public class TelegramBotBackgroundService(
        ILogger<TelegramBotBackgroundService> logger,
        DatabaseServices dbService,
        BotInteractionService botInteractionService,
        ReverseMarkdown.Converter reverseMarkdownConverter)
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
                var broadcastMessages = await dbService.GetBroadcastMessagesAsync();
                User[]? users = null;
                if (broadcastMessages.Length >0)
                {
                    users = await dbService.GetUsersAsync();
                }
                

                foreach (var message in broadcastMessages)
                {
                    var markdown = reverseMarkdownConverter.Convert(message.Message);
                    foreach (var user in users!)
                    {
                        if (user.NotSendBroadcastMessages) continue;
                        var messageText = string.Format($"Привет {user.FirstName}\r\n {markdown}");
                        await botInteractionService.SendCustomMessage(user.Id, messageText, token);
                    }
                    message.Sent = true;
                    await dbService.MessageUpdateAsync(message, token);
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
