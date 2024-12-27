using Microsoft.EntityFrameworkCore;

namespace TelegramBot;

public class Startup(IServiceProvider serviceProvider)
{
    private static void SetParams()
    {
        AppContext.SetSwitch("System.Globalization.Invariant", true);
        TimeZoneInfo.ClearCachedData();
    }

    private static void DoMigrate(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
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

    public void Initialize(string[] args)
    {
        if (args.Length > 0 && args[0] == "delete")
            DeleteDatabase();
        SetParams();
        DoMigrate(serviceProvider);
    }

    private void DeleteDatabase()
    {
        using var scope = serviceProvider.CreateScope();
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