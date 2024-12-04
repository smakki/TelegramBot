using Microsoft.EntityFrameworkCore;
using TelegramBot.Models;

namespace TelegramBot
{
    public class DatabaseServices(IServiceScopeFactory scopeFactory)
    {
        public async Task AddUserTaskAsync(long telegramId, UserTask task, CancellationToken token)
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
            return dbService.UserTasks.Where(obj => obj.TaskDate <= tenSecond & !obj.Remindered).ToList();
        }

        public List<UserTask> GetTasksForNotification(CancellationToken token)
        {
            using var scope = scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            var tenSecond = DateTime.UtcNow.AddSeconds(10);
            return dbService.UserTasks.Where(obj => obj.ReminderDate <= tenSecond & !obj.Completed & !obj.Notificated).ToList();
        }

        public async Task AddUser(long telegramId, string name, CancellationToken token)
        {
            using var scope = scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            
            dbService.Users.AddIfNotExists(new User { Name = name, Id = telegramId },obj=>obj.Id ==telegramId);
            await dbService.SaveChangesAsync(token);
        }

        public List<UserTask> GetActualUserTasks(long telegramId, CancellationToken token)
        {
            using var scope = scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            return dbService.UserTasks
                .Include(task =>task.User )
                .Where(obj => obj.TelegramId == telegramId & !obj.Completed)
                .ToList();
        }
        
        public User? GetUserById(long telegramId, CancellationToken token)
        {
            using var scope = scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            return dbService.Users.FirstOrDefault(obj => obj.Id == telegramId);
        }
    }
}
