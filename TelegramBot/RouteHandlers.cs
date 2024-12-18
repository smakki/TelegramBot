using System.Text.Json;
using Microsoft.AspNetCore.Http;
using TelegramBot.Models;

namespace TelegramBot;

public class RouteHandlers(DatabaseServices dbService)
{
    public async Task<string> GetStatisticHandler(HttpContext _)
    {
        return JsonSerializer.Serialize(await dbService.GetUserTaskStatisticsAsync());
    }
    
    public async Task<string> GetUsersHandler(HttpContext _)
    {
        return JsonSerializer.Serialize(await dbService.GetUsersAsync());
    }

    public async Task PostMessageHandler(BroadcastMessage message)
    {
        await dbService.AddMessageAsync(message);
    }
}