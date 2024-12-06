using System.ComponentModel.DataAnnotations;

namespace TelegramBot.Models
{
    public class UserTask
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }
        [MaxLength(100)]
        public string? Message { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime TaskDate { get; set; }
        public DateTime ReminderDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public int NumberOfSnoozes { get; set; }
        public bool Reminded { get; set; }
        public bool Notified { get; set; }
        
        public bool Completed { get; set; }

        public long UserId { get; set; }
        public User? User { get; set; }
        
        public UserTask(User user, string message, DateTime taskDate)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone);
            User = user;
            UserId = user.Id;
            TelegramId = user.Id;
            Message = message;
            TaskDate = TimeZoneInfo.ConvertTimeToUtc(taskDate, timeZone);
            ReminderDate = TimeZoneInfo.ConvertTimeToUtc(taskDate.AddMinutes(-user.ReminderToTaskMinutes), timeZone);
            AddedDate = DateTime.UtcNow;
            
        }

        public UserTask() { }
    }
}