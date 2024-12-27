using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TelegramBot.Models
{
    public class User
    {
        public long Id { get; set; }
        
        [MaxLength(100)]
        public string? Username { get; set; }
        
        [MaxLength(100)]
        public string? FirstName { get; set; }
        
        [MaxLength(100)]
        public string? LastName { get; set; }
        
        public List<LocationHistory> Locations { get; set; } = new List<LocationHistory>();
        
        [MaxLength(100)]
        public string TimeZone {get; set;} = "Europe/Moscow";

        public int ReminderToTaskMinutes{get; set;} = 60;
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
        public List<UserTask>? Tasks { get; set; } = [];
        public bool NotSendBroadcastMessages { get; set; } = false;
    }

}
