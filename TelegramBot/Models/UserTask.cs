namespace TelegramBot
{
    public class UserTask
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }
        public string? Message { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime TaskDate { get; set; }
        public DateTime ReminderDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public int NumberOfSnoozes { get; set; }
        public bool Remindered { get; set; }
        public bool Notificated { get; set; }
        public bool Completed { get; set; }

        public UserTask(long telegramId, string message, DateTime taskDate)
        {
            TelegramId = telegramId;
            Message = message;
            TaskDate = taskDate.ToUniversalTime();
            ReminderDate = taskDate.AddHours(-1).ToUniversalTime();
            AddedDate = DateTime.UtcNow.ToUniversalTime();
        }

        public UserTask() { }
    }
}