namespace TelegramBot
{
    internal class Texts
    {
        public static Telegram.Bot.Types.Enums.ParseMode parseMode = Telegram.Bot.Types.Enums.ParseMode.Markdown;
        public static string StartMessage = """
            👋 Привет! Я — твой To-Do бот 📋, который понимает тебя с полуслова!
            
            ✨ Что я умею: 
            ✅ Создавать задачи и распознавать даты прямо из текста — напиши, что и когда нужно сделать.
            ⏰ Напоминать о важных делах, чтобы ничего не забыть.
            📊 Показывать твою статистику по выполненным задачам.
            Попробуй прямо сейчас: напиши что-нибудь вроде «`Позвонить врачу завтра в 10:00`», и я сразу всё запишу!
            Давай начнем? 🚀
            """;
        public static string StartMessage2 = """
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
        public static string[] StartMessages = new string[] { StartMessage, StartMessage2 };
        public static string ClarificationMessage = "Добавить задачу с именем {0} на дату {1}?";
        public static string NotificationAddedSuccess = """
            ✅ Задача добавлена!

            🗒 Что делать: {0}
            📅 Когда: {1}

            ⏰ Я напомню об этом вовремя! 💪
            """;
        public static string TaskReminderMessage = """
            ⏳ Напоминание о задаче!

            🗒 Задача: Позвонить маме
            📅 Когда: Через 1 час

            💡 Что дальше?
            """;
        public static string TaskNotificationMessage = """
            ⏰ Время пришло!

            🗒 Задача: Позвонить маме
            📅 Сейчас

            💡 Что сделаем?
            """;
        public static string CancelAddingTask = "❌ Задача не добавлена.\r\n\r\nЕсли передумаешь, просто напиши её заново! 😊";
        public static string TaskFromTasksList = """
            {0} 🗒 {1}
            📅 Когда: {2}
            """;
    }
}
