using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using TelegramBot.Models;

namespace TelegramBot
{
    public class TelegramBotDbContext(DbContextOptions<TelegramBotDbContext> options) : DbContext(options)
    {
        public DbSet<UserTask> UserTasks { get; init; }

        public DbSet<User> Users { get; init; }
        public DbSet<BroadcastMessage> BroadcastMessages { get; init; }
        
        public DbSet<LocationHistory> LocationHistory { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade); 
            
            modelBuilder.Entity<LocationHistory>()
                .HasOne(loc => loc.User)
                .WithMany(u => u.Locations)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
    public static class DbSetExtensions
    {
        public static EntityEntry<T> AddIfNotExists<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, bool>>? predicate = null) where T : class, new()
        {
            var exists = predicate != null ? dbSet.Any(predicate) : dbSet.Any();
            return !exists ? dbSet.Add(entity) : null!;
        }

        public static EntityEntry<T> AddOrUpdateIfExists<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, bool>>? predicate = null) where T : class, new()
        {
            var exists = predicate != null ? dbSet.Any(predicate) : dbSet.Any();
            return !exists ? dbSet.Add(entity) : dbSet.Update(entity);
        }
    }
}
