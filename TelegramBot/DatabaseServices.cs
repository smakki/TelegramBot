using Hors.Models;
using Microsoft.EntityFrameworkCore;

namespace TelegramBot
{
    public class DatabaseServices
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public DatabaseServices(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public  UserTask AddUserTaskAsync(long TelegramId, HorsParseResult Task, CancellationToken token)
        {
            var TaskDate = Task.Dates[0].DateFrom.ToUniversalTime();
            var task = new UserTask
            {
                TelegramId = TelegramId,
                TaskDate = TaskDate,
                Message = Task.Text,
                AddedDate = DateTime.UtcNow.ToUniversalTime(),
                NotificationDate = TaskDate.AddHours(-1).ToUniversalTime(),
            };
            using var scope = _scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            dbService.UserTasks.Add(task);
            dbService.SaveChangesAsync(token);
            return task;
        }

        public async Task TaskNotificatedAsync(UserTask Task, CancellationToken token)
        {
            Task.Notificated = true;
            using var scope = _scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            dbService.Update(Task);
            dbService.SaveChanges();
        }

        public List<UserTask> GetUpcomingTasks(CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            var tenSecond = DateTime.UtcNow.AddSeconds(10);
            return dbService.UserTasks.Where(obj => obj.NotificationDate <= tenSecond & !obj.Notificated).ToList();
        }

        public async Task AddUser(long TelegramId, string Name, CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            
            dbService.Users.AddIfNotExists(new Users { Name = Name, Id = TelegramId },obj=>obj.Id ==TelegramId);
            dbService.SaveChanges();
        }
    }
}
