﻿namespace TelegramBot;

internal static class Texts
{
    public static readonly Telegram.Bot.Types.Enums.ParseMode ParseMode = Telegram.Bot.Types.Enums.ParseMode.Markdown;

    private static readonly string StartMessage = """
                                                   👋 Привет! Я — твой To-Do бот 📋, который понимает тебя с полуслова!

                                                   ✨ Что я умею: 
                                                   ✅ Создавать задачи и распознавать даты прямо из текста — напиши, что и когда нужно сделать.
                                                   ⏰ Напоминать о важных делах, чтобы ничего не забыть.
                                                   📊 Показывать твою статистику по выполненным задачам.
                                                   Попробуй прямо сейчас: напиши что-нибудь вроде «`Позвонить врачу завтра в 10:00`», и я сразу всё запишу!
                                                   Давай начнем? 🚀
                                                   """;

    private static readonly string StartMessage2 = """
                                                    👋 Привет! Я — твой To-Do бот 📋, который понимает задачи, даже если ты их просто напишешь текстом!

                                                    ✨ Что я умею:
                                                    ✅ Распознавать даты и создавать задачи прямо из текста.
                                                    ⏰ Напоминать о делах, чтобы ничего не забывалось.
                                                    📊 Вести статистику выполненных задач.

                                                    Попробуй вот так:
                                                    `Сходить за продуктами завтра в 18:00`  
                                                    `Позвонить маме через 5 минут` 
                                                    `Написать отчет в пятницу в 14:00`  
                                                    `Забрать посылку в течение часа`  
                                                    Напиши свою задачу, и я сразу всё запишу. Давай начнем? 🚀
                                                    """;

    public static readonly string[] StartMessages = [StartMessage, StartMessage2];
    
    public static readonly string NotificationSaveSuccess = """
                                                            ✅ Задача успешно сохранена!

                                                            🗒 Что делать: `{0}`
                                                            📅 Когда: `{1}`

                                                            ⏰ Я напомню об этом вовремя! 💪
                                                            """;

    public static readonly string TaskReminderMessage = """
                                                        ⏳ Напоминание о задаче!

                                                        🗒 Задача: `{0}`
                                                        📅 Когда: Через 1 час

                                                        💡 Что дальше?
                                                        """;

    public static readonly string TaskNotificationMessage = """
                                                            ⏰ Время пришло!

                                                            🗒 Задача: `{0}`
                                                            📅 Сейчас

                                                            💡 Что сделаем?
                                                            """;

    public static readonly string CancelAddingTask =
        "❌ Задача не добавлена.\r\n\r\nЕсли передумаешь, просто напиши её заново! 😊";

    public static readonly string Task = """
                                         🗒 `{0}`
                                         📅 Когда: `{1}`

                                         💡 Что делаем?
                                         """;

    public static readonly string EdTask = "✏️ Редактировать";
    public static readonly string Add = "✔️ Добавить";
    public static readonly string Cancel = "🚫 Отменить";
    public static readonly string DelTask = "🗑️ Удалить";

    public static readonly string CompletedTask = """
                                                  🎉 Задача выполнена!

                                                  🗒 Что сделано: `{0}`
                                                  📊 Отличная работа! Продолжай в том же духе 💪
                                                  """;

    public static readonly string EditTask = """
                                             ✏️ Редактирование задачи

                                             📌 Текущая задача:
                                             🗒 Что делать: `{0}`
                                             📅 Когда: `{1}`

                                             💡  Выбери, что ты хочешь изменить:
                                             """;

    public static readonly string EnterDate = """
                                              📅 Введите новую дату и время для задачи

                                              Пожалуйста, напишите дату и время в формате:
                                              `27.12.2024 08:30`  
                                              Если передумали, нажмите ❌ Отмена.
                                              """;
    
    public static readonly string UnknownDate = """
                                              Не удалось распознать дату
                                              """;

    public static readonly string SettingsMessage = """
                                                    ⚙️ Настройки учетной записи

                                                    1️⃣ Часовой пояс
                                                    🌍 Текущий: {0}
                                                    Измените часовой пояс, чтобы напоминания приходили в правильное время.

                                                    2️⃣ Интервал предварительного напоминания
                                                    ⏳ Текущий: {1}
                                                    Настройте, за сколько времени до выполнения задачи вы хотите получать предварительное напоминание.

                                                    💡 Выберите настройку, которую хотите изменить:

                                                    🔘 Изменить часовой пояс
                                                    🔘 Изменить интервал напоминания

                                                    ❌ Выйти из настроек
                                                    """;

}