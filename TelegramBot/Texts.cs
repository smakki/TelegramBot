namespace TelegramBot;

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
                                                            ⏰ Предварительное напоминание: `{2}`

                                                            💡 Я напомню об этом вовремя! 💪
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
    public static readonly string Cancel = "❌ Отмена";
    public static readonly string DelTask = "🗑️ Удалить";

    public static readonly string CompletedTask = """
                                                  🎉 Задача выполнена!

                                                  🗒 Что сделано: *{0}*
                                                  📊 Отличная работа! Продолжай в том же духе 💪
                                                  """;

    public static readonly string EditTask = """
                                             ✏️ Редактирование задачи

                                             📌 Текущая задача:
                                             🗒 Что делать: *{0}*
                                             📅 Когда: *{1}*

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
                                                    🌍 Текущий: *{0}*
                                                    Измените часовой пояс, чтобы напоминания приходили в правильное время.

                                                    2️⃣ Интервал предварительного напоминания
                                                    ⏳ Текущий: *{1}*
                                                    Настройте, за сколько времени до выполнения задачи вы хотите получать предварительное напоминание.

                                                    💡 Выберите настройку, которую хотите изменить:

                                                    🔘 Изменить часовой пояс
                                                    🔘 Изменить интервал напоминания

                                                    ❌ Выйти из настроек
                                                    """;

    public static readonly string TimeZoneChangeMessage = """
                                                          🌍 Изменение часового пояса

                                                          Выберите, как вы хотите настроить часовой пояс:
                                                          1️⃣ Отправьте сдвиг относительно UTC, например:
                                                          *+3*  или *-5*  
                                                          2️⃣ Отправьте свою геолокацию, и я автоматически определю ваш часовой пояс.

                                                          Если передумали, нажмите ❌ Отмена.
                                                          """;

    public static readonly string SendLocation = "📍 Отправить геолокацию";
    public static readonly string Snooze = "🔁 Напомнить позже";
    public static readonly string Close = "❌ Закрыть уведомление";
    public static readonly string Pass = "⏩ Пропустить";
    public static readonly string Complete = "✅ Выполнено";

    public static readonly string DateNotParse = """
                                                 ⚠️ Ошибка!

                                                 Не удалось распознать введенную дату. Убедитесь, что вы указали её в правильном формате:

                                                 `27.12.2024 08:30` 
                                                 Попробуйте ещё раз или нажмите ❌ Отмена, чтобы выйти.
                                                 """;

    public static readonly string NoTasks = """
                                            📂 Ваш список задач пуст.

                                            Вы пока не добавили ни одной задачи. Напишите, что нужно сделать, например:

                                            Копировать код
                                            `Позвонить маме завтра в 18:00`  
                                            И я сразу всё запишу! ✨
                                            """;

    public static readonly string YourTasks = "📂 Ваши задачи: \n";
    public static readonly string TimeZone = "🌍 Часовой пояс";
    public static readonly string ReminderInterval = "⏳ Интервал напоминания";
    public static readonly string Yes = "✔️ Да";
    public static readonly string No = "❌ Нет";
    public static readonly string YourTimeZone = "Ваш часовой пояс: \r\n {0} ?";
    
    public static readonly string TaskText = "📝 Текст задачи";
    public static readonly string TaskDate = "📅 Дату и время";
    public static readonly string Request = "🔄 Всё(новый запрос)";
    public static readonly string Save = "💾 Сохранить изменения";


    // public static readonly string
}