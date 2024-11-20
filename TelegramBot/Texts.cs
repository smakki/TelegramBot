namespace TelegramBot
{
    internal class Texts
    {
        public static Telegram.Bot.Types.Enums.ParseMode parseMode = Telegram.Bot.Types.Enums.ParseMode.Markdown;

        public static string StartMessage = "👋 Привет! Я — твой To-Do бот 📋, который понимает тебя с полуслова!\r\n\r\n" +
            "✨ Что я умею:\r\n" +
            "✅ Создавать задачи и распознавать даты прямо из текста — напиши, что и когда нужно сделать.\r\n" +
            "⏰ Напоминать о важных делах, чтобы ничего не забыть.\r\n" +
            "📊 Показывать твою статистику по выполненным задачам.\r\n\r\n" +
            "Попробуй прямо сейчас: напиши что-нибудь вроде «`Позвонить врачу завтра в 10:00`», и я сразу всё запишу!\r\n\r\n" +
            "Давай начнем? 🚀";

        public static string ClarificationMessage = "Добавить задачу с именем {0} на дату {1}?";
    }
}
