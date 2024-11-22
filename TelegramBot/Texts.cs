﻿namespace TelegramBot
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

        public static string NotificationMessage = """
            ⏰ Напоминание!

            🗒 Задача: {0}
            📅 Время: Сейчас

            Действуй! Ты справишься 💪
            """;
    }
}
