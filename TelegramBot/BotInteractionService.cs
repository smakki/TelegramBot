using Hors;
using Hors.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class BotInteractionService
    {
        private readonly ITelegramBotClient _client;
        private readonly DatabaseServices _dbService;
        private Dictionary<Guid, HorsParseResult> userSessions = new();
        public BotInteractionService(ITelegramBotClient client, DatabaseServices dbService)
        {
            _client = client;
            _dbService = dbService;
        }
        private async Task SendStartMessage(long chatId, CancellationToken token)
        {
            var startMessage = Texts.StartMessages[new Random().Next(Texts.StartMessages.Length)];
            await _client.SendMessage(chatId, startMessage, Texts.parseMode,replyMarkup:new ReplyKeyboardRemove(),cancellationToken:token);
        }

        public async Task SendNotification(UserTask Task, CancellationToken token)
        {
            var Notifacation = $"{Task.Message} в {Task.TaskDate}";
            await _client.SendMessage(Task.TelegramId, Notifacation, Texts.parseMode, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: token);
        }

        private async Task SendClarificationMessage(long chatId, CancellationToken token, HorsParseResult task)
        {
            var queryId = Guid.NewGuid();
            var TaskName = task.Text;
            var TaskDate = task.Dates.First().DateFrom;
            InlineKeyboardMarkup InlineKeyboard = new(
                new InlineKeyboardButton() { Text = "Да", CallbackData = $"yes-add-task:{queryId}" },
                new InlineKeyboardButton() { Text = "Нет", CallbackData = $"no-add-task{queryId}" }
                );
            userSessions.Add(queryId, task);
            await _client.SendMessage(chatId, String.Format(Texts.ClarificationMessage, TaskName, TaskDate), Texts.parseMode, replyMarkup: InlineKeyboard,cancellationToken:token);
            
        }
        
        public async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
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

        private async Task CallbackQueryHandler(CallbackQuery query, CancellationToken token)
        {
            switch (query.Data)
            {
                case var s when s.StartsWith("yes-add-task"):
                    var queryId = s.Split(':')[1];
                    await _dbService.AddUserTaskAsync(query.Message.Chat.Id, userSessions[Guid.Parse(queryId)],token);
                    await _client.EditMessageText(query.Message.Chat.Id,messageId:query.Message.Id,text:query.Message.Text, replyMarkup: null, cancellationToken: token);
                    break;
                default:
                    break;
            }
        }

        private async Task MessageTextHandler(Message message, CancellationToken token)
        {
            if (message.Text is not { } text)
                return;

            var chatId = message.Chat.Id;

            switch (text)
            {
                case "/start":
                    await SendStartMessage(chatId, token);
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
