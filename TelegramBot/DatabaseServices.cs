using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TelegramBot.Models;
using User = TelegramBot.Models.User;

namespace TelegramBot;

public class DatabaseServices(IServiceScopeFactory scopeFactory)
{
    public async Task AddUserTaskAsync(UserTask task, CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();
        task.User = null;
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        dbService.UserTasks.AddOrUpdateIfExists(task, userTask => userTask.Id == task.Id);
        await dbService.SaveChangesAsync(token);
    }

    public async Task TaskUpdateAsync(UserTask task, CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        dbService.Update(task);
        await dbService.SaveChangesAsync(token);
    }

    public async Task UserUpdateAsync(User user, CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        dbService.Update(user);
        await dbService.SaveChangesAsync(token);
    }

    public async Task TaskDeleteAsync(UserTask task, CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        dbService.UserTasks.Remove(task);
        await dbService.SaveChangesAsync(token);
    }

    public List<UserTask> GetTasksForReminder(CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        var tenSecond = DateTime.UtcNow.AddSeconds(10);
        return dbService.UserTasks.Where(obj => (obj.ReminderDate <= tenSecond) & !obj.Reminded & !obj.Completed)
            .ToList();
    }

    public List<UserTask> GetTasksForNotification(CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        var tenSecond = DateTime.UtcNow.AddSeconds(10);
        return dbService.UserTasks.Where(obj => (obj.TaskDate <= tenSecond) & !obj.Completed & !obj.Notified).ToList();
    }

    public async Task AddUser(long telegramId, Chat chat, CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();

        dbService.Users.AddIfNotExists(
            new User
            {
                Username = chat.Username, FirstName = chat.FirstName, LastName = chat.LastName, Id = telegramId
            }, obj => obj.Id == telegramId);
        await dbService.SaveChangesAsync(token);
    }

    public async Task AddLocationHistory(long telegramId, Location location, CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        dbService.LocationHistory.Add(new LocationHistory
        {
            Longitude = location.Longitude, Latitude = location.Latitude, UserId = telegramId
        });

        await dbService.SaveChangesAsync(token);
    }

    public List<UserTask> GetActualUserTasks(long telegramId, CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        return dbService.UserTasks
            .Include(task => task.User)
            .Where(obj => (obj.TelegramId == telegramId) & !obj.Completed)
            .ToList();
    }

    public User? GetUserById(long telegramId, CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        return dbService.Users.FirstOrDefault(obj => obj.Id == telegramId);
    }

    public async Task<List<UserTaskStatisticsDto>> GetUserTaskStatisticsAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        return await dbService.Users
            .Select(user => new UserTaskStatisticsDto
            {
                UserId = user.Id,
                UserName = user.Username,
                Name = user.FirstName + " " + user.LastName,
                TotalTasks = user.Tasks == null ? 0 : user.Tasks.Count,
                CompletedTasks = user.Tasks == null ? 0 : user.Tasks.Count(task => task.Completed),
                LastCompletedTaskDate = user.Tasks == null ? DateTime.MinValue : user.Tasks
                    .Where(task => task.Completed)
                    .OrderByDescending(task => task.CompletionDate)
                    .Select(task => task.CompletionDate)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    public async Task<User[]> GetUsersAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        return await dbService.Users.ToArrayAsync();
    }

    public async Task AddMessageAsync(BroadcastMessage message)
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        await dbService.BroadcastMessages.AddAsync(message);
        await dbService.SaveChangesAsync();
    }
    public async Task<BroadcastMessage[]> GetBroadcastMessagesAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        return await dbService.BroadcastMessages.Where((message) => !message.Sent).ToArrayAsync();
    }
    
    public async Task MessageUpdateAsync(BroadcastMessage message, CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
        dbService.Update(message);
        await dbService.SaveChangesAsync(token);
    }
}