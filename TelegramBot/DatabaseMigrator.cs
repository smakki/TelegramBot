using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace TelegramBot
{
    public class DatabaseMigrator
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseMigrator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Migrate()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();

            try
            {
                var pendingMigrations = dbContext.Database.GetPendingMigrations();

                if (!pendingMigrations.Any()) return;
                Console.WriteLine("Using migrations...");
                dbContext.Database.Migrate();
                Console.WriteLine("Migrations successfully applied.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying migrations: {ex.Message}");
                throw;
            }
        }

        public void DeleteDatabase()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
            try
            {
                dbContext.Database.EnsureDeleted();
                Console.WriteLine("db is clear");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting db: {ex.Message}");
                throw;
            }
        }
    }
}