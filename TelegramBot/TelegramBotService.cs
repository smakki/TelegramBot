using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot.Options;

namespace TelegramBot
{
    public class TelegramBotService(
        ILogger<TelegramBotService> logger,
        IOptions<TelegramOptions> telegramOptions,
        BotInteractionService botInteractionService)
        : BackgroundService
    {
        private readonly TelegramOptions _options = telegramOptions.Value;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bot = new TelegramBotClient(_options.Token);
            var commands = new[] {
                new BotCommand { Command = "/start",Description = "Старт бота"},
                new BotCommand() { Command = "/mytasks",Description = "Мои задачи"},
                new BotCommand() { Command = "/settings",Description = "Настройки"},
                new BotCommand() { Command = "/help",Description = "Помощь"}
            };
            await bot.SetMyCommands(commands, cancellationToken: stoppingToken);
            logger.LogInformation("bot started");
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = []
            };
            while (!stoppingToken.IsCancellationRequested)
            {
                await bot.ReceiveAsync(
                    updateHandler: UpdateHandler,
                    errorHandler: ErrorHandlerAsync,
                    receiverOptions: receiverOptions,
                    cancellationToken: stoppingToken);
            }
        }

        private Task ErrorHandlerAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            switch (exception)
            {
                case ApiRequestException apiRequestException:
                    var messageError = $"Telegram API error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}";
                    logger.LogError(apiRequestException, messageError);
                    return Task.CompletedTask;
                default:
                    messageError = exception.ToString();
                    logger.LogError(messageError);
                    return Task.CompletedTask;
            }

        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var handler = update switch
            {
                { Message: { } message } => botInteractionService.MessageTextHandler(message, token),
                { CallbackQuery: { } query } => botInteractionService.CallbackQueryHandler(query, token),
                _ => botInteractionService.UnknownMessageHandler(update, token)
            };
            await handler;
        }
    }
}
