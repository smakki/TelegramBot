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
        public BotInteractionService(ITelegramBotClient client)
        {
            _client = client;
        }
        private async Task SendStartMessage(long chatId, CancellationToken token)
        {
            await _client.SendMessage(chatId, Texts.StartMessage, Texts.parseMode,replyMarkup:new ReplyKeyboardRemove());
            
        }

        private async Task SendClarificationMessage(long chatId, CancellationToken token, HorsParseResult task)
        {
            var TaskName = task.Text;
            var TaskDate = task.Dates.First().DateFrom;
            InlineKeyboardMarkup InlineKeyboard = new(
                new InlineKeyboardButton() { Text = "Да", CallbackData = "yes-add-task" },
                new InlineKeyboardButton() { Text = "Нет", CallbackData = "no-add-task" }
                );
            await _client.SendMessage(chatId, String.Format(Texts.ClarificationMessage, TaskName, TaskDate), Texts.parseMode, replyMarkup: InlineKeyboard);
        }
        private Task AddUserTaskAsync() 
        { 
            return Task.CompletedTask; 
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
                case "yes-add-task":
                    await AddUserTaskAsync();
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
