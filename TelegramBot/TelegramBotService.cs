using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot.Options;

namespace TelegramBot
{
    public class TelegramBotService : BackgroundService
    {
        private readonly ILogger<TelegramBotService> _logger;
        private readonly BotInteractionService _botInteractionService;
        private readonly TelegramOptions _options;

        public TelegramBotService(ILogger<TelegramBotService> logger, IOptions<TelegramOptions> telegramOptions, BotInteractionService botInteractionService)
        {
            _logger = logger;
            _options = telegramOptions.Value;
            _botInteractionService = botInteractionService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            var bot = new TelegramBotClient(_options.Token);
            var commands = new BotCommand[] { 
                new BotCommand() { Command = "/mytasks",Description = "Мои задачи"},
                new BotCommand() { Command = "/settings",Description = "Настройки"},
                new BotCommand() { Command = "/help",Description = "Помощь"}
            };
            await bot.SetMyCommands(commands, cancellationToken: stoppingToken);
            _logger.LogInformation("bot started");
            ReceiverOptions recieverOptions = new()
            {
                AllowedUpdates = []
            };
            while (!stoppingToken.IsCancellationRequested)
            {
                await bot.ReceiveAsync(
                    updateHandler: UpdateHandler,
                    errorHandler: ErrorHandlerAsync,
                    receiverOptions: recieverOptions,
                    cancellationToken: stoppingToken);
            }
        }

        private Task ErrorHandlerAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            switch (exception)
            {
                case ApiRequestException apiRequestException:
                    var messageError = $"Telegram API error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}";
                    _logger.LogError(apiRequestException, messageError);
                    return Task.CompletedTask;
                default:
                    messageError = exception.ToString();
                    _logger.LogError(messageError);
                    return Task.CompletedTask;
            }

        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            await _botInteractionService.UpdateHandler(client, update, token);
        }
    }
}
