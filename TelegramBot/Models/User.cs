using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TelegramBot.Models
{
    public class User
    {
        public long Id { get; set; }
        
        [MaxLength(100)]
        public string Name { get; set; }
        
        [MaxLength(100)]
        public string TimeZone {get; set;}
        
        public int ReminderToTaskMinutes{get; set;}
        
        public List<UserTask>? Tasks { get; set; } = new ();
        public User()
        {
            TimeZone = "Europe/Moscow";
            ReminderToTaskMinutes = 60;
        }
    }
}
