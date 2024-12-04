using System.Net.Mime;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

public class Keyboards
{
    public static IReplyMarkup? GetKeyboard(string name, string queryId = "")
    {
        switch (name)
        {
            case "Reminder":
                return new InlineKeyboardMarkup(
                    new InlineKeyboardButton() { Text = Texts.Snooze, CallbackData = $"retry-reminder" },
                    new InlineKeyboardButton() { Text = Texts.Close, CallbackData = $"close-reminder" }
                );
            
            case "Notification":
                return new InlineKeyboardMarkup(
                    new InlineKeyboardButton() { Text = Texts.Complete, CallbackData = $"complete-task:{queryId}" },
                    new InlineKeyboardButton() { Text = Texts.Pass, CallbackData = $"pass-task:{queryId}" },
                    new InlineKeyboardButton() { Text = Texts.DelTask, CallbackData = $"delete-task:{queryId}" }
                );
            
            case "Clarification":
                return new InlineKeyboardMarkup([
                    [
                        new InlineKeyboardButton() { Text = Texts.Add, CallbackData = $"yes-add-task:{queryId}" },
                        new InlineKeyboardButton() { Text = Texts.Cancel, CallbackData = $"no-add-task:{queryId}" }
                    ],
                    [
                        new InlineKeyboardButton() { Text = Texts.EdTask, CallbackData = $"edit-task:{queryId}" }
                    ]
                ]);
            
            case "Geolocation":
                return new ReplyKeyboardMarkup(
                    new KeyboardButton(Texts.SendLocation) { RequestLocation = true })
                {
                    ResizeKeyboard = true,
                    OneTimeKeyboard = true
                };
            
            case "ActualTask":
                return new InlineKeyboardMarkup(
                    new InlineKeyboardButton() { Text = Texts.EdTask, CallbackData = $"edit-task:{queryId}" },
                    new InlineKeyboardButton() { Text = Texts.DelTask, CallbackData = $"delete-task:{queryId}" }
                );
            
            case "Settings":
                return new InlineKeyboardMarkup(
                    new InlineKeyboardButton() { Text = Texts.TimeZone, CallbackData = $"settings-timezone" },
                    new InlineKeyboardButton() { Text = Texts.ReminderInterval, CallbackData = $"settings-interval-reminder" }
                );
            
            case "ConfirmTimeZone":
                return new InlineKeyboardMarkup(
                    new InlineKeyboardButton() { Text = Texts.Yes, CallbackData = $"yes-edit-timezone:{queryId}" },
                    new InlineKeyboardButton() { Text = Texts.No, CallbackData = $"no-edit-timezone:{queryId}" }
                );
            
            case "EditTask":
                return new InlineKeyboardMarkup([
                    [
                        new InlineKeyboardButton { Text = Texts.TaskText, CallbackData = $"edit-text:{queryId}" },
                        new InlineKeyboardButton { Text = Texts.TaskDate, CallbackData = $"edit-date:{queryId}" }
                    ],
                    [
                        new InlineKeyboardButton { Text = Texts.Request, CallbackData = $"edit-request:{queryId}" },
                        new InlineKeyboardButton { Text = Texts.Cancel, CallbackData = $"edit-cancel:{queryId}" }
                    ],
                    [
                        new InlineKeyboardButton { Text = Texts.Save, CallbackData = $"edit-save:{queryId}" }
                    ]
                ]);
            
            case "Remove":
                return new ReplyKeyboardRemove();
            
            default:
                return null;
        }
    }

   
}