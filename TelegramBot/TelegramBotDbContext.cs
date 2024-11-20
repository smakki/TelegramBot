using Microsoft.EntityFrameworkCore;

namespace TelegramBot
{
    internal class TelegramBotDbContext: DbContext
    {
        public TelegramBotDbContext(DbContextOptions<TelegramBotDbContext> options)
            : base(options)
        {

        }
        public DbSet<UserTask> UserTasks { get; set; }
    }
}
