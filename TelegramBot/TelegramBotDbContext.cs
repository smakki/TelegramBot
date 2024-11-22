using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace TelegramBot
{
    public class TelegramBotDbContext: DbContext
    {
        public TelegramBotDbContext(DbContextOptions<TelegramBotDbContext> options)
            : base(options)
        {

        }
        public DbSet<UserTask> UserTasks { get; set; }

        
    }
}
