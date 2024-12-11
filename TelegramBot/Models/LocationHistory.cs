namespace TelegramBot.Models;

public class LocationHistory
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
}