namespace TelegramBot.Models;

public class UserTaskStatisticsDto
{
    public long UserId { get; set; }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public DateTime? LastCompletedTaskDate { get; set; }
}