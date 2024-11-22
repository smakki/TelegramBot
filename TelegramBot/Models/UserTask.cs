namespace TelegramBot
{
    public class UserTask
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public long TelegramId { get; set; }
        public string Message { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime TaskDate { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool Notificated { get; set; }
    }
}