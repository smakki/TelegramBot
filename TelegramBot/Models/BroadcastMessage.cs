using System.ComponentModel.DataAnnotations;

namespace TelegramBot.Models;

public class BroadcastMessage
{
    public int Id { get; set; }
    [MaxLength(1000)]
    public string? Message { get; set; }
    public DateTime AddedDate { get; set; }  = DateTime.UtcNow;
    public DateTime SendingDate { get; set; } = DateTime.UtcNow;
    public bool Sent { get; set; }
}