using Hors;
using Hors.Models;
using System.Collections.Concurrent;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;




namespace TelegramBot
{
    public class BotInteractionService
    {
        private readonly ITelegramBotClient _client;
        private readonly DatabaseServices _dbService;
        private readonly ConcurrentDictionary<string, UserTask> usersTasks = [];
        private readonly ConcurrentDictionary<long, string> usersSteps = [];
        public BotInteractionService(ITelegramBotClient client, DatabaseServices dbService)
        {
            _client = client;
            _dbService = dbService;
        }
        private async Task SendStartMessage(long chatId, CancellationToken token)
        {
            var startMessage = Texts.StartMessages[new Random().Next(Texts.StartMessages.Length)];
            await _client.SendMessage(chatId, startMessage, Texts.parseMode, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: token);
        }

        public async Task SendReminder(UserTask Task, CancellationToken token)
        {
            InlineKeyboardMarkup InlineKeyboard = new(
                new InlineKeyboardButton() { Text = "🔁 Напомнить позже", CallbackData = $"retry-reminder" },
                new InlineKeyboardButton() { Text = "❌ Закрыть уведомление", CallbackData = $"close-reminder" }
            );
            var Notifacation = String.Format(Texts.TaskReminderMessage, Task.Message);
            await _client.SendMessage(Task.TelegramId, Notifacation, Texts.parseMode, replyMarkup: InlineKeyboard, cancellationToken: token);
        }

        public async Task SendNotification(UserTask Task, CancellationToken token)
        {
            var queryId = Guid.NewGuid().ToString();
            InlineKeyboardMarkup InlineKeyboard = new(
                new InlineKeyboardButton() { Text = "✅ Выполнено", CallbackData = $"complete-task:{queryId}" },
                new InlineKeyboardButton() { Text = "⏩ Пропустить", CallbackData = $"no-complete-task:{queryId}" },
                new InlineKeyboardButton() { Text = Texts.DelTask, CallbackData = $"delete-task:{queryId}" }
            );
            usersTasks.TryAdd(queryId, Task);
            var Notifacation = String.Format(Texts.TaskNotificationMessage, Task.Message);
            await _client.SendMessage(Task.TelegramId, Notifacation, Texts.parseMode, replyMarkup: InlineKeyboard, cancellationToken: token);
        }

        private async Task SendClarificationMessage(long chatId, CancellationToken token, HorsParseResult task)
        {
            if (task.Dates.Count == 0)
            {
                await _client.SendMessage(chatId, "Не удалось распознать дату в запросе", Texts.parseMode, replyMarkup: null, cancellationToken: token);
                return;
            }
            var queryId = Guid.NewGuid().ToString();
            var TaskName = task.Text;
            var TaskDate = task.Dates.First().DateFrom;
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[] {
                new InlineKeyboardButton() { Text = Texts.Add, CallbackData = $"yes-add-task:{queryId}" },
                new InlineKeyboardButton() { Text = Texts.Cancel, CallbackData = $"no-add-task:{queryId}" },
            },new[]{
                new InlineKeyboardButton() { Text = Texts.EdTask, CallbackData = $"edit-task:{queryId}" }
            }
            });
            usersTasks.TryAdd(queryId, new UserTask(chatId, TaskName, TaskDate));
            await _client.SendMessage(chatId, String.Format(Texts.Task, TaskName, TaskDate), Texts.parseMode, replyMarkup: inlineKeyboard, cancellationToken: token);
        }

        private void SendActualTasksMessage(long chatId, CancellationToken token)
        {

            var num = 1;
            var tasks = _dbService.GetActualUserTasks(chatId, token);
            foreach (var task in tasks)
            {
                var queryId = Guid.NewGuid().ToString();
                usersTasks.TryAdd(queryId, task);
                InlineKeyboardMarkup InlineKeyboard = new(
                new InlineKeyboardButton() { Text = Texts.EdTask, CallbackData = $"edit-task:{queryId}" },
                new InlineKeyboardButton() { Text = Texts.DelTask, CallbackData = $"delete-task:{queryId}" }
                );

                if (num == 1) _client.SendMessage(chatId, "🗂 Ваши задачи:", cancellationToken: token);
                _client.SendMessage(chatId,
                   String.Format(Texts.Task, task.Message, task.TaskDate),
                   Texts.parseMode,
                   replyMarkup: InlineKeyboard,
                   cancellationToken: token
                   );
                Task.Delay(300, token);
                num++;
            };
        }

        public async Task UpdateHandler(Update update, CancellationToken token)
        {
            var handler = update switch
            {
                { Message: { } message } => MessageTextHandler(message, token),
                { CallbackQuery: { } query } => CallbackQueryHandler(query, token),
                _ => UnknownMessageHandler(update, token),
            };
            await handler;
        }

        private async Task UnknownMessageHandler(Update update, CancellationToken token)
        {
            Console.WriteLine("Unknown message type");
        }
        public async Task SaveTask(string queryId,int messageId, CancellationToken token)
        {
            var UserTask = usersTasks[queryId];
            await _dbService.AddUserTaskAsync(UserTask.TelegramId, UserTask, token);
            await _client.EditMessageText(
                UserTask.TelegramId,
                messageId: messageId,
                text: String.Format(Texts.NotificationSaveSuccess, UserTask.Message, UserTask.ReminderDate),
                Texts.parseMode,
                replyMarkup: null,
                cancellationToken: token);
        }
        private async Task CallbackQueryHandler(CallbackQuery query, CancellationToken token)
        {
            if (query.Message == null || query.Data == null) return;
            long chatId = query.Message.Chat.Id;
            int messageId = query.Message.Id;
            switch (query.Data)
            {
                case var s when s.StartsWith("yes-add-task"):
                    var queryId = s.Split(':')[1];
                    await SaveTask(queryId, messageId, token);
                    break;
                case var s when s.StartsWith("no-add-task"):
                    queryId = s.Split(':')[1];
                    usersTasks.TryRemove(queryId, out _);
                    await _client.EditMessageText(
                        chatId,
                        messageId: messageId,
                        text: Texts.CancelAddingTask,
                        replyMarkup: null,
                        cancellationToken: token);
                    break;
                case var s when s.StartsWith("complete-task"):
                    queryId = s.Split(':')[1];
                    var UserTask = usersTasks[queryId];
                    UserTask.Completed = true;
                    UserTask.CompletionDate = DateTime.Now.ToUniversalTime();
                    await _dbService.TaskUpdateAsync(UserTask, token);
                    await _client.EditMessageText(
                        chatId,
                        messageId: messageId,
                        text: String.Format(Texts.CompletedTask, UserTask.Message),
                        replyMarkup: null,
                        cancellationToken: token);
                    usersTasks.TryRemove(queryId, out _);
                    break;
                case var s when s.StartsWith("edit-task"):
                    queryId = s.Split(':')[1];
                    await EditTask(chatId, queryId, messageId, token);
                    break;
                case var s when s.StartsWith("edit-text"):
                    queryId = s.Split(':')[1];
                    usersSteps.TryAdd(chatId, $"edit-text:{queryId}:{messageId}");
                    await _client.SendMessage(chatId, "Введите новый текст");
                    break;
                case var s when s.StartsWith("edit-date"):
                    queryId = s.Split(':')[1];
                    usersSteps.TryAdd(chatId, $"edit-date:{queryId}:{messageId}");
                    await _client.SendMessage(chatId, "Введите новую дату в формате `27.12.2024 08:30`", Texts.parseMode);
                    break;
                case var s when s.StartsWith("edit-save"):
                    queryId = s.Split(':')[1];
                    await SaveTask(queryId, messageId, token);
                    break;
                default:

                    break;
            }
        }

        private async Task EditTask(long chatId, string queryId, int messageId, CancellationToken token)
        {
            var UserTask = usersTasks[queryId];
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {new[]{
                            new InlineKeyboardButton { Text = "📝 Текст задачи", CallbackData = $"edit-text:{queryId}" },
                            new InlineKeyboardButton { Text = "📅 Дату и время", CallbackData = $"edit-date:{queryId}" }
            },
                new[]{
                            new InlineKeyboardButton { Text = "🔄 Всё(новый запрос)", CallbackData = $"edit-request:{queryId}" },
                            new InlineKeyboardButton { Text = "❌ Отмена", CallbackData = $"edit-cancel:{queryId}" }
                },
                new[]{
                            new InlineKeyboardButton { Text = "💾 Сохранить изменения", CallbackData = $"edit-save:{queryId}" },
                },
                    });
            await _client.EditMessageText(chatId, messageId, String.Format(Texts.EditTask, UserTask.Message, UserTask.TaskDate), Texts.parseMode, replyMarkup: inlineKeyboard, cancellationToken: token);
        }
        private async Task MessageTextHandler(Message message, CancellationToken token)
        {
            if (message.Text is not { } text)
                return;

            var chatId = message.Chat.Id;
            if (usersSteps.TryGetValue(chatId, out string nextStep))
            {
                switch (nextStep)
                {
                    case var s when s.StartsWith("edit-text"):
                        usersSteps.TryRemove(chatId, out _);
                        var queryId = s.Split(':')[1];
                        var messageId = s.Split(':')[2];
                        var task = usersTasks[queryId];
                        task.Message = text;
                        await _client.DeleteMessage(chatId, message.Id, token);
                        await _client.DeleteMessage(chatId, message.Id - 1, token);
                        await EditTask(chatId, queryId, int.Parse(messageId), token);
                        break;
                    case var s when s.StartsWith("edit-date"):
                        usersSteps.TryRemove(chatId, out _);
                        queryId = s.Split(':')[1];
                        messageId = s.Split(':')[2];
                        task = usersTasks[queryId];
                        CultureInfo provider = CultureInfo.InvariantCulture;
                        if (DateTime.TryParse(text, out DateTime result))
                        {
                            task.TaskDate = result.ToUniversalTime();
                            await _client.DeleteMessage(chatId, message.Id, token);
                            await _client.DeleteMessage(chatId, message.Id - 1, token);
                            await EditTask(chatId, queryId, int.Parse(messageId), token);
                        }
                        else
                        {
                            await _client.SendMessage(chatId, "Не удалось распознать дату");
                        }
                        break;
                }
                return;
            }

            switch (text)
            {
                case "/start":
                    await _dbService.AddUser(chatId, message.Chat.Username, token);
                    await SendStartMessage(chatId, token);
                    break;
                case "/mytasks":
                    SendActualTasksMessage(chatId, token);
                    break;
                default:
                    var Hors = new HorsTextParser();
                    var result = Hors.Parse(text, DateTime.Now);
                    await SendClarificationMessage(chatId, token, result);
                    break;
            }
        }
    }
}
