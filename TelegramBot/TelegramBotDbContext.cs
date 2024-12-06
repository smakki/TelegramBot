using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using TelegramBot.Models;

namespace TelegramBot
{
    public class TelegramBotDbContext : DbContext
    {
        public TelegramBotDbContext(DbContextOptions<TelegramBotDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }


        public DbSet<UserTask> UserTasks { get; init; }

        public DbSet<User> Users { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи "один ко многим" между User и UserTask
            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление задач при удалении пользователя
        }
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
