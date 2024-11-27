using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using TelegramBot.Models;

namespace TelegramBot
{
    public class TelegramBotDbContext(DbContextOptions<TelegramBotDbContext> options) : DbContext(options)
    {
        public DbSet<UserTask> UserTasks { get; init; }

        public DbSet<Users> Users { get; init; }


    }
    public static class DbSetExtensions
    {
        public static EntityEntry<T> AddIfNotExists<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, bool>> predicate = null) where T : class, new()
        {
            var exists = predicate != null ? dbSet.Any(predicate) : dbSet.Any();
            return !exists ? dbSet.Add(entity) : null;
        }

        public static EntityEntry<T> AddOrUpdateIfExists<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, bool>> predicate = null) where T : class, new()
        {
            var exists = predicate != null ? dbSet.Any(predicate) : dbSet.Any();
            return !exists ? dbSet.Add(entity) : dbSet.Update(entity);
        }
    }
}
