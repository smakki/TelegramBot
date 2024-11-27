using Hors;
using Hors.Models;
using System.Collections.Concurrent;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;


namespace TelegramBot;

public class BotInteractionService(DatabaseServices dbService, TelegramUtils utils)
{
    private readonly ConcurrentDictionary<string, UserTask> _usersTasks = [];
    private readonly ConcurrentDictionary<long, string> _usersSteps = [];

    private async Task SendStartMessage(long chatId, CancellationToken token)
    {
        var startMessage = Texts.StartMessages[new Random().Next(Texts.StartMessages.Length)];
        await utils.SendTextMessage(chatId, startMessage, new ReplyKeyboardRemove(), token);
    }

    public async Task SendReminder(UserTask task, CancellationToken token)
    {
        InlineKeyboardMarkup inlineKeyboard = new(
            new InlineKeyboardButton() { Text = "🔁 Напомнить позже", CallbackData = $"retry-reminder" },
            new InlineKeyboardButton() { Text = "❌ Закрыть уведомление", CallbackData = $"close-reminder" }
        );
        var notification = string.Format(Texts.TaskReminderMessage, task.Message);
        await utils.SendTextMessage(task.TelegramId, notification, inlineKeyboard, token);
    }

    public async Task SendNotification(UserTask task, CancellationToken token)
    {
        var queryId = Guid.NewGuid().ToString();
        InlineKeyboardMarkup inlineKeyboard = new(
            new InlineKeyboardButton() { Text = "✅ Выполнено", CallbackData = $"complete-task:{queryId}" },
            new InlineKeyboardButton() { Text = "⏩ Пропустить", CallbackData = $"no-complete-task:{queryId}" },
            new InlineKeyboardButton() { Text = Texts.DelTask, CallbackData = $"delete-task:{queryId}" }
        );
        _usersTasks.TryAdd(queryId, task);
        var notification = string.Format(Texts.TaskNotificationMessage, task.Message);
        await utils.SendTextMessage(task.TelegramId, notification, inlineKeyboard, token);
    }

    private async Task SendClarificationMessage(long chatId, HorsParseResult task, CancellationToken token)
    {
        if (task.Dates.Count == 0)
        {
            await utils.SendTextMessage(chatId, "Не удалось распознать дату в запросе", null, token);
            return;
        }

        var queryId = Guid.NewGuid().ToString();
        var taskName = task.Text;
        var taskDate = task.Dates.First().DateFrom;
        var inlineKeyboard = new InlineKeyboardMarkup([
            [
                new InlineKeyboardButton() { Text = Texts.Add, CallbackData = $"yes-add-task:{queryId}" },
                new InlineKeyboardButton() { Text = Texts.Cancel, CallbackData = $"no-add-task:{queryId}" }
            ],
            [
                new InlineKeyboardButton() { Text = Texts.EdTask, CallbackData = $"edit-task:{queryId}" }
            ]
        ]);
        _usersTasks.TryAdd(queryId, new UserTask(chatId, taskName, taskDate));
        var message = string.Format(Texts.Task, taskName, taskDate);
        await utils.SendTextMessage(chatId, message, inlineKeyboard, token);
    }

    private async Task SendActualTasksMessage(long chatId, CancellationToken token)
    {
        var tasks = dbService.GetActualUserTasks(chatId, token);
        if (tasks.Count == 0)
        {
            await utils.SendTextMessage(chatId, "У вас нет задач.", null, token);
            return;
        }

        await Task.WhenAll(tasks.Select<UserTask, Task>((task, index) =>
        {
            var queryId = Guid.NewGuid().ToString();
            _usersTasks.TryAdd(queryId, task);
            InlineKeyboardMarkup inlineKeyboard = new(
                new InlineKeyboardButton() { Text = Texts.EdTask, CallbackData = $"edit-task:{queryId}" },
                new InlineKeyboardButton() { Text = Texts.DelTask, CallbackData = $"delete-task:{queryId}" }
            );

            var taskText = index == 0 ? "🗂 Ваши задачи:\n" : string.Empty;
            taskText += string.Format(Texts.Task, task.Message, task.TaskDate);

            return utils.SendTextMessage(chatId, taskText, inlineKeyboard, token);
        }));
    }

    public async Task UpdateHandler(Update update, CancellationToken token)
    {
        var handler = update switch
        {
            { Message: { } message } => MessageTextHandler(message, token),
            { CallbackQuery: { } query } => CallbackQueryHandler(query, token),
            _ => UnknownMessageHandler(update, token)
        };
        await handler;
    }

    private async Task UnknownMessageHandler(Update update, CancellationToken token)
    {
        if (update.Message != null) await utils.SendMessageAdmin("Unknown message \r\n" + update.Message.Text, token);
    }

    private async Task SaveTask(string queryId, int messageId, CancellationToken token)
    {
        var userTask = _usersTasks[queryId];
        await dbService.AddUserTaskAsync(userTask.TelegramId, userTask, token);
        var message = string.Format(Texts.NotificationSaveSuccess, userTask.Message, userTask.ReminderDate);
        await utils.EditTextMessage(userTask.TelegramId, messageId, message, null, token);
    }

    private async Task CallbackQueryHandler(CallbackQuery query, CancellationToken token)
    {
        if (query.Message == null || query.Data == null) return;
        var chatId = query.Message.Chat.Id;
        var messageId = query.Message.Id;
        switch (query.Data)
        {
            case var s when s.StartsWith("yes-add-task"):
                var queryId = s.Split(':')[1];
                await SaveTask(queryId, messageId, token);
                break;
            case var s when s.StartsWith("no-add-task"):
                queryId = s.Split(':')[1];
                _usersTasks.TryRemove(queryId, out _);
                await utils.EditTextMessage(chatId,messageId,Texts.CancelAddingTask,null, token);
                break;
            case var s when s.StartsWith("complete-task"):
                queryId = s.Split(':')[1];
                var userTask = _usersTasks[queryId];
                userTask.Completed = true;
                userTask.CompletionDate = DateTime.Now.ToUniversalTime();
                await dbService.TaskUpdateAsync(userTask, token);
                var message = string.Format(Texts.CompletedTask, userTask.Message);
                await utils.EditTextMessage(chatId, messageId, message, null, token);
                _usersTasks.TryRemove(queryId, out _);
                break;
            case var s when s.StartsWith("edit-task"):
                queryId = s.Split(':')[1];
                await EditTask(chatId, queryId, messageId, token);
                break;
            case var s when s.StartsWith("edit-text"):
                queryId = s.Split(':')[1];
                _usersSteps.TryAdd(chatId, $"edit-text:{queryId}:{messageId}");
                await utils.SendTextMessage(chatId, "Введите новый текст", null, token);
                break;
            case var s when s.StartsWith("edit-date"):
                queryId = s.Split(':')[1];
                _usersSteps.TryAdd(chatId, $"edit-date:{queryId}:{messageId}");
                await utils.SendTextMessage(chatId, Texts.EnterDate, null, token);
                break;
            case var s when s.StartsWith("edit-save"):
                queryId = s.Split(':')[1];
                await SaveTask(queryId, messageId, token);
                break;
            default:
                await utils.SendMessageAdmin("Неизвесный Callback query \n\r" + query.Data, token);
                break;
        }
    }

    private async Task EditTask(long chatId, string queryId, int messageId, CancellationToken token)
    {
        var userTask = _usersTasks[queryId];
        var inlineKeyboard = new InlineKeyboardMarkup([
            [
                new InlineKeyboardButton { Text = "📝 Текст задачи", CallbackData = $"edit-text:{queryId}" },
                new InlineKeyboardButton { Text = "📅 Дату и время", CallbackData = $"edit-date:{queryId}" }
            ],
            [
                new InlineKeyboardButton { Text = "🔄 Всё(новый запрос)", CallbackData = $"edit-request:{queryId}" },
                new InlineKeyboardButton { Text = "❌ Отмена", CallbackData = $"edit-cancel:{queryId}" }
            ],
            [
                new InlineKeyboardButton { Text = "💾 Сохранить изменения", CallbackData = $"edit-save:{queryId}" }
            ]
        ]);
        var message = string.Format(Texts.EditTask, userTask.Message, userTask.TaskDate);
        await utils.EditTextMessage(chatId, messageId, message, inlineKeyboard, token);
    }

    private async Task MessageTextHandler(Message message, CancellationToken token)
    {
        if (message.Text is not { } text)
            return;

        var chatId = message.Chat.Id;
        if (_usersSteps.TryGetValue(chatId, out var nextStep))
        {
            switch (nextStep)
            {
                case var _ when nextStep.StartsWith("edit-text"):
                    _usersSteps.TryRemove(chatId, out _);
                    var queryId = nextStep.Split(':')[1];
                    var messageId = nextStep.Split(':')[2];
                    var task = _usersTasks[queryId];
                    task.Message = text;
                    await utils.DeleteMessage(chatId, message.Id, token);
                    await utils.DeleteMessage(chatId, message.Id - 1, token);
                    await EditTask(chatId, queryId, int.Parse(messageId), token);
                    break;
                case var _ when nextStep.StartsWith("edit-date"):
                    _usersSteps.TryRemove(chatId, out _);
                    queryId = nextStep.Split(':')[1];
                    messageId = nextStep.Split(':')[2];
                    task = _usersTasks[queryId];
                    if (DateTime.TryParse(text, out var result))
                    {
                        task.TaskDate = result.ToUniversalTime();
                        await utils.DeleteMessage(chatId, message.Id, token);
                        await utils.DeleteMessage(chatId, message.Id - 1, token);
                        await EditTask(chatId, queryId, int.Parse(messageId), token);
                    }
                    else
                    {
                        await utils.SendTextMessage(chatId,Texts.UnknownDate,null,token);
                    }
                    break;
            }
            return;
        }

        switch (text)
        {
            case "/start":
                if (message.Chat.Username != null) await dbService.AddUser(chatId, message.Chat.Username, token);
                await SendStartMessage(chatId, token);
                break;
            case "/mytasks":
                await SendActualTasksMessage(chatId, token);
                break;
            default:
                var hors = new HorsTextParser();
                var result = hors.Parse(text, DateTime.Now);
                await SendClarificationMessage(chatId, result, token);
                break;
        }
    }
}