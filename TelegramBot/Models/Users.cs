using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TelegramBot.Models
{
    public class Users
    {
        public long Id { get; set; }
        
        [MaxLength(100)]
        public string Name { get; set; }
        
        [MaxLength(100)]
        [DefaultValue("getutcdate()")]
        public string? TimeZone {get; set;}
        
        public int ReminderToTaskMinutes{get; set;}

        public Users()
        {
            TimeZone = "+3";
            ReminderToTaskMinutes = 60;
        }
    }
}
