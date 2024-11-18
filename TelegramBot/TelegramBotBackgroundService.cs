using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot.Options;

namespace TelegramBot
{
    public class TelegramBotBackgroundService : BackgroundService
    {
        private readonly ILogger<TelegramBotBackgroundService> _logger;
        private readonly TelegramOptions _options;

        public TelegramBotBackgroundService(ILogger<TelegramBotBackgroundService> logger, IOptions<TelegramOptions> telegramOptions)
        {
            _logger = logger;
            _options = telegramOptions.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bot = new TelegramBotClient(_options.Token);
            ReceiverOptions recieverOptions = new()
            {
                AllowedUpdates = []
            };
            while (!stoppingToken.IsCancellationRequested)
            {
                await bot.ReceiveAsync(
                    updateHandler: updateHandler,
                    errorHandler: errorHandler,
                    receiverOptions:recieverOptions,
                    cancellationToken:stoppingToken);

            }
        }

        private Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _=>exception.ToString()
            };
            _logger.LogError(exception, errorMessage);
            return Task.CompletedTask;
        }

        private async Task updateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message is not { } message)
                return;

            if (message.Text is not { } text)
                return;
            var chatId = message.Chat.Id;

            Console.WriteLine($"Recieved a {text} message in chat {chatId}");
            Message sentMessage = await client.SendMessage(chatId, "Your said : " + text,cancellationToken:token);
        }
    }
}
