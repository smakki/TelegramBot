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
        public bool Remindered { get; set; }
        public bool Notificated { get; set; }
        public bool Completed { get; set; }

        public long UserId { get; set; }
        public User? User { get; set; }
        
        public UserTask(User user, string message, DateTime taskDate)
        {
            var TimeZone = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone);
            User = user;
            UserId = user.Id;
            TelegramId = user.Id;
            Message = message;
            TaskDate = TimeZoneInfo.ConvertTimeToUtc(taskDate, TimeZone);
            ReminderDate = TimeZoneInfo.ConvertTimeToUtc(taskDate.AddMinutes(-user.ReminderToTaskMinutes), TimeZone);
            AddedDate = DateTime.UtcNow;
            
            // TaskDate = TimeZoneInfo.ConvertTimeFromUtc(taskDate, TimeZone);
            // ReminderDate = TimeZoneInfo.ConvertTimeFromUtc(taskDate.AddMinutes(-user.ReminderToTaskMinutes), TimeZone);
            // AddedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
        }

        public UserTask() { }
    }
}