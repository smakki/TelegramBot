using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Options;

namespace TelegramBot;

public class TelegramUtils(ITelegramBotClient client, IOptions<TelegramOptions> telegramOptions)
{
    private readonly TelegramOptions _options = telegramOptions.Value;

    public async Task SendTextMessage(long chatId, string message, IReplyMarkup? markup = null,
        CancellationToken token = default)
    {
        await client.SendMessage(chatId, message, Texts.ParseMode, replyMarkup: markup, cancellationToken: token);
    }

    public async Task EditTextMessage(long chatId, int messageId, string message, IReplyMarkup? markup = null,
        CancellationToken token = default)
    {
        await client.EditMessageText(chatId, messageId, message, Texts.ParseMode,
            replyMarkup: (InlineKeyboardMarkup?)markup, cancellationToken: token);
    }

    public async Task SendMessageAdmin(string message, CancellationToken token)
    {
        await client.SendMessage(_options.AdminId, message, Texts.ParseMode, cancellationToken: token);
    }
    public async Task DeleteMessage(long chatId, int messageId, CancellationToken token = default)
    {
        await client.DeleteMessage(chatId, messageId, token);
    } 
    
}