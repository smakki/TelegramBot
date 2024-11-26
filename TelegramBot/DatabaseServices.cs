namespace TelegramBot
{
    public class DatabaseServices
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public DatabaseServices(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public async Task AddUserTaskAsync(long TelegramId, UserTask Task, CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            dbService.UserTasks.AddOrUpdateIfExists(Task);
            await dbService.SaveChangesAsync(token);
        }

        public async Task TaskUpdateAsync(UserTask Task, CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            dbService.Update(Task);
            await dbService.SaveChangesAsync(token);
        }

        public List<UserTask> GetTasksForReminder(CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            var tenSecond = DateTime.UtcNow.AddSeconds(10);
            return dbService.UserTasks.Where(obj => obj.TaskDate <= tenSecond & !obj.Remindered).ToList();
        }

        public List<UserTask> GetTasksForNotification(CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            var tenSecond = DateTime.UtcNow.AddSeconds(10);
            return dbService.UserTasks.Where(obj => obj.ReminderDate <= tenSecond & !obj.Completed & !obj.Notificated).ToList();
        }

        public async Task AddUser(long TelegramId, string Name, CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            
            dbService.Users.AddIfNotExists(new Users { Name = Name, Id = TelegramId },obj=>obj.Id ==TelegramId);
            await dbService.SaveChangesAsync(token);
        }

        public List<UserTask> GetActualUserTasks(long TelegramId, CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            return dbService.UserTasks.Where(obj => obj.TelegramId == TelegramId & !obj.Completed).ToList();
        }
    }
}
