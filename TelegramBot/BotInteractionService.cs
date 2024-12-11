using Hors;
using Hors.Models;
using System.Collections.Concurrent;
using Humanizer;
using Telegram.Bot.Types;
using TelegramBot.Models;

namespace TelegramBot;

public class BotInteractionService(DatabaseServices dbService, TelegramUtils utils, DateTimeUtils dateTimeUtils)
{
    private readonly ConcurrentDictionary<string, UserTask> _usersTasks = [];
    private readonly ConcurrentDictionary<long, string> _usersSteps = [];
    private readonly Dictionary<string, Func<CallbackQuery, CancellationToken, Task>> _callbackHandlers;

    #region Sends

    private async Task SendStartMessage(long chatId, CancellationToken token)
    {
        var startMessage = Texts.StartMessages[new Random().Next(Texts.StartMessages.Length)];
        await utils.SendTextMessage(chatId, startMessage, Keyboards.GetKeyboard("Remove"), token);
    }

    public async Task SendReminder(UserTask task, CancellationToken token)
    {
        var queryId = Guid.NewGuid().ToString();
        _usersTasks.TryAdd(queryId, task);
        var notification = string.Format(Texts.TaskReminderMessage, task.Message,task.TaskDate.Humanize());
        await utils.SendTextMessage(task.TelegramId, notification, Keyboards.GetKeyboard("Reminder",queryId), token);
    }

    private async Task SendChangeTimeZoneMessage(long chatId, CancellationToken token)
    {
        _usersSteps.TryAdd(chatId, $"settings-timezone");
        await utils.SendTextMessage(chatId, Texts.TimeZoneChangeMessage, Keyboards.GetKeyboard("Geolocation"), token);
    }

    private async Task SendChangeIntervalReminderMessage(long chatId, CancellationToken token)
    {
        var user = dbService.GetUserById(chatId, token);
        if (user != null)
        {
            _usersSteps.TryAdd(chatId, $"settings-interval-reminder");
            var message = string.Format(Texts.EditReminderInterval,
                DateTimeUtils.GetPresentInterval(user.ReminderToTaskMinutes));
            await utils.SendTextMessage(chatId, message, Keyboards.GetKeyboard("cancel"), token);
        }
    }

    private async Task SendConfirmTimeZoneMessage(long chatId, Location location, CancellationToken token)
    {
        await dbService.AddLocationHistory(chatId, location, token);
        var tz = DateTimeUtils.GetTimeZoneByGeoLocation(location.Latitude, location.Longitude).Result;
        _usersSteps.TryRemove(chatId, out _);
        var mes = string.Format(Texts.YourTimeZone, tz.DisplayName);
        await utils.SendTextMessage(chatId, mes, Keyboards.GetKeyboard("ConfirmTimeZone", tz.Id),
            token);
    }

    private async Task SendConfirmIntervalMessage(long chatId, int interval, int messageId, CancellationToken token)
    {
        var user = dbService.GetUserById(chatId, token);
        if (user != null)
        {
            user.ReminderToTaskMinutes = interval;
            await dbService.UserUpdateAsync(user, token);
            await SendSettingsMessage(chatId, messageId, token);
        }
    }

    private async Task SendUnknownIntervalMessage(long chatId, CancellationToken token)
    {
        _usersSteps.TryAdd(chatId, $"settings-interval-reminder");
        await utils.SendTextMessage(chatId, Texts.UnknownInterval, Keyboards.GetKeyboard("Cancel"), token);
    }

    public async Task SendNotification(UserTask task, CancellationToken token)
    {
        var queryId = Guid.NewGuid().ToString();
        _usersTasks.TryAdd(queryId, task);
        var notification = string.Format(Texts.TaskNotificationMessage, task.Message);
        await utils.SendTextMessage(task.TelegramId, notification,
            Keyboards.GetKeyboard("Notification", queryId), token);
    }

    private async Task SendClarificationMessage(long chatId, HorsParseResult task, CancellationToken token)
    {
        if (task.Dates.Count == 0)
        {
            await utils.SendTextMessage(chatId, Texts.DateNotParse, null, token);
            return;
        }

        var queryId = Guid.NewGuid().ToString();
        var taskName = task.Text;
        var taskDate = task.Dates.First().DateFrom;

        var user = dbService.GetUserById(chatId, token);
        _usersTasks.TryAdd(queryId, new UserTask(user, taskName, taskDate));
        var message = string.Format(Texts.Task, taskName, taskDate);
        await utils.SendTextMessage(chatId, message, Keyboards.GetKeyboard("Clarification", queryId), token);
    }

    private async Task SendActualTasksMessage(long chatId, CancellationToken token)
    {
        var tasks = dbService.GetActualUserTasks(chatId, token);
        if (tasks.Count == 0)
        {
            await utils.SendTextMessage(chatId, Texts.NoTasks, null, token);
            return;
        }

        await Task.WhenAll(tasks.Select<UserTask, Task>((task, index) =>
        {
            var queryId = Guid.NewGuid().ToString();
            _usersTasks.TryAdd(queryId, task);
            var taskText = index == 0 ? Texts.YourTasks : string.Empty;
            taskText += string.Format(Texts.Task, task.Message,
                TimeZoneInfo.ConvertTimeFromUtc(task.TaskDate,
                    TimeZoneInfo.FindSystemTimeZoneById(task.User.TimeZone)));

            return utils.SendTextMessage(chatId, taskText, Keyboards.GetKeyboard("ActualTask", queryId), token);
        }));
    }

    private async Task SendSettingsMessage(long chatId, int messageId = 0, CancellationToken token = default)
    {
        var user = dbService.GetUserById(chatId, token);
        if (user != null)
        {
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone);

            var message = string.Format(Texts.SettingsMessage, timezone.DisplayName,
                DateTimeUtils.GetPresentInterval(user.ReminderToTaskMinutes));
            if (messageId != 0)
                await utils.EditTextMessage(chatId, messageId, message, Keyboards.GetKeyboard("Settings"), token);
            else
                await utils.SendTextMessage(chatId, message, Keyboards.GetKeyboard("Settings"), token);
        }
        else
        {
            await utils.SendTextMessage(chatId, "Не нашли пользователя", null, token);
        }
    }

    #endregion

    #region Handlers

    public async Task UnknownMessageHandler(Update update, CancellationToken token)
    {
        if (update.Message != null) await utils.SendMessageAdmin("Unknown message \r\n" + update.Message.Text, token);
    }

    public async Task CallbackQueryHandler(CallbackQuery query, CancellationToken token)
    {
        var queryId = "";
        if (query.Message == null || query.Data == null) return;
        var chatId = query.Message.Chat.Id;
        var messageId = query.Message.Id;
        if (query.Data.Split(':').Length == 2) queryId = query.Data.Split(':')[1];

        switch (query.Data)
        {
            case var s when s.StartsWith("yes-add-task"):
                await SaveTask(queryId, messageId, token);
                break;
            case var s when s.StartsWith("no-add-task"):
                _usersTasks.TryRemove(queryId, out _);
                await utils.EditTextMessage(chatId, messageId, Texts.CancelAddingTask, null, token);
                break;
            case var s when s.StartsWith("complete-task"):
                var userTask = _usersTasks[queryId];
                userTask.Completed = true;
                userTask.CompletionDate = DateTime.Now.ToUniversalTime();
                await dbService.TaskUpdateAsync(userTask, token);
                var message = string.Format(Texts.CompletedTask, userTask.Message);
                await utils.EditTextMessage(chatId, messageId, message, null, token);
                _usersTasks.TryRemove(queryId, out _);
                break;
            case var s when s.StartsWith("edit-task"):
                await EditTask(chatId, queryId, messageId, token);
                break;
            case var s when s.StartsWith("edit-text"):
                _usersSteps.TryAdd(chatId, $"edit-text:{queryId}:{messageId}");
                message = string.Format(Texts.EnterTaskText, _usersTasks[queryId].Message);
                await utils.SendTextMessage(chatId, message, Keyboards.GetKeyboard("Cancel", queryId), token);
                break;
            case var s when s.StartsWith("edit-date"):
                _usersSteps.TryAdd(chatId, $"edit-date:{queryId}:{messageId}");
                userTask = _usersTasks[queryId];
                message = string.Format(Texts.EnterDate,
                    TimeZoneInfo.ConvertTimeFromUtc(userTask.TaskDate,
                        TimeZoneInfo.FindSystemTimeZoneById(userTask.User.TimeZone)));
                await utils.SendTextMessage(chatId, message, Keyboards.GetKeyboard("Cancel", queryId), token);
                break;
            case var s when s.StartsWith("edit-save"):
                await SaveTask(queryId, messageId, token);
                break;
            case var s when s.StartsWith("edit-cancel"):
                if (_usersTasks.TryGetValue(queryId, out var task))
                {
                    _usersSteps.TryRemove(chatId, out _);
                    message = string.Format(Texts.Task, task.Message, task.TaskDate);
                    await utils.EditTextMessage(chatId, messageId, message,
                        Keyboards.GetKeyboard("Clarification", queryId), token);
                }

                break;
            case var s when s.StartsWith("delete-task"):
                await dbService.TaskDeleteAsync(_usersTasks[queryId], token);
                await utils.DeleteMessage(chatId, messageId, token);
                break;
            case var s when s.StartsWith("settings-timezone"):
                await SendChangeTimeZoneMessage(chatId, token);
                break;
            case var s when s.StartsWith("settings-interval-reminder"):
                await SendChangeIntervalReminderMessage(chatId, token);
                break;
            case var s when s.StartsWith("yes-edit-timezone"):
                var user = dbService.GetUserById(chatId, token);
                user.TimeZone = queryId;
                await dbService.UserUpdateAsync(user, token);
                await utils.DeleteMessage(chatId, messageId, token);
                await utils.DeleteMessage(chatId, messageId - 1, token);
                await utils.DeleteMessage(chatId, messageId - 2, token);
                await SendSettingsMessage(chatId, messageId - 3, token);
                break;
            case var s when s.StartsWith("retry-reminder"):
                userTask = _usersTasks[queryId];
                userTask.Reminded = false;
                userTask.ReminderDate = userTask.ReminderDate.AddMinutes(
                    ((userTask.TaskDate - userTask.ReminderDate).TotalMinutes / 2)
                );
                userTask.NumberOfSnoozes++;
                await dbService.TaskUpdateAsync(userTask, token);
                break;
            case var s when s.StartsWith("cancel"):
                await utils.DeleteMessage(chatId, messageId, token);
                _usersSteps.TryRemove(chatId, out _);
                break;
            default:
                await utils.SendMessageAdmin("Неизвесный Callback query \n\r" + query.Data, token);
                break;
        }
    }

    public async Task MessageTextHandler(Message message, CancellationToken token)
    {
        if (message.Location is { } location) await LocationHandler(message, location, token);
        if (message.Text is not { } text)
            return;

        var chatId = message.Chat.Id;
        if (_usersSteps.TryGetValue(chatId, out var nextStep))
        {
            var nextStepParams = nextStep.Split(':');
            var messageId = int.Parse(nextStepParams.Length > 2 ? nextStepParams[2] : "0");
            switch (nextStep)
            {
                case var _ when nextStep.StartsWith("edit-text"):
                    _usersSteps.TryRemove(chatId, out _);
                    var queryId = nextStep.Split(':')[1];
                    var task = _usersTasks[queryId];
                    task.Message = text;
                    await utils.DeleteMessage(chatId, message.Id, token);
                    await utils.DeleteMessage(chatId, message.Id - 1, token);
                    await EditTask(chatId, queryId, messageId, token);
                    break;
                case var _ when nextStep.StartsWith("edit-date"):
                    _usersSteps.TryRemove(chatId, out _);
                    queryId = nextStep.Split(':')[1];
                    task = _usersTasks[queryId];
                    if (DateTime.TryParse(text, out var result))
                    {
                        task.TaskDate = TimeZoneInfo.ConvertTimeToUtc(result,
                            TimeZoneInfo.FindSystemTimeZoneById(task.User.TimeZone));
                        task.ReminderDate = TimeZoneInfo.ConvertTimeToUtc(
                            result.AddMinutes(-task.User.ReminderToTaskMinutes),
                            TimeZoneInfo.FindSystemTimeZoneById(task.User.TimeZone));
                        await utils.DeleteMessage(chatId, message.Id, token);
                        await utils.DeleteMessage(chatId, message.Id - 1, token);
                        await EditTask(chatId, queryId, messageId, token);
                    }
                    else
                    {
                        await utils.SendTextMessage(chatId, Texts.DateNotParse, null, token);
                    }

                    break;
                case var _ when nextStep.StartsWith("settings-interval-reminder"):
                    _usersSteps.TryRemove(chatId, out _);
                    if (int.TryParse(text, out var interval))
                        await SendConfirmIntervalMessage(chatId, interval, messageId, token);
                    else
                        await SendUnknownIntervalMessage(chatId, token);

                    break;
                case "settings-timezone":

                    break;
            }

            return;
        }

        switch (text)
        {
            case "/start":
                await dbService.AddUser(chatId, message.Chat, token);
                await SendStartMessage(chatId, token);
                break;
            case "/mytasks":
                await SendActualTasksMessage(chatId, token);
                break;
            case "/settings":
                await SendSettingsMessage(chatId, 0, token);
                break;
            default:
                var user = dbService.GetUserById(chatId, token);
                var hors = new HorsTextParser();
                var result = hors.Parse(text,
                    TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone)));
                await SendClarificationMessage(chatId, result, token);
                break;
        }
    }

    private async Task LocationHandler(Message message, Location location, CancellationToken token)
    {
        var chatId = message.Chat.Id;
        if (_usersSteps.TryGetValue(message.Chat.Id, out var nextStep))
            if (nextStep == "settings-timezone")
                await SendConfirmTimeZoneMessage(chatId, location, token);
    }

    #endregion

    private async Task SaveTask(string queryId, int messageId, CancellationToken token)
    {
        var userTask = _usersTasks[queryId];
        var userZone = TimeZoneInfo.FindSystemTimeZoneById(userTask.User.TimeZone);
        await dbService.AddUserTaskAsync(userTask.TelegramId, userTask, token);
        var message = string.Format(Texts.NotificationSaveSuccess, userTask.Message,
            TimeZoneInfo.ConvertTimeFromUtc(userTask.TaskDate, userZone),
            TimeZoneInfo.ConvertTimeFromUtc(userTask.ReminderDate, userZone));
        await utils.EditTextMessage(userTask.TelegramId, messageId, message, null, token);
    }

    private async Task EditTask(long chatId, string queryId, int messageId, CancellationToken token)
    {
        var userTask = _usersTasks[queryId];

        var message = string.Format(Texts.EditTask, userTask.Message,
            TimeZoneInfo.ConvertTimeFromUtc(userTask.TaskDate,
                TimeZoneInfo.FindSystemTimeZoneById(userTask.User.TimeZone)));
        await utils.EditTextMessage(chatId, messageId, message, Keyboards.GetKeyboard("EditTask", queryId), token);
    }
}