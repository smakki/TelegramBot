using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace TelegramBot;

public class RouteHandlers(DatabaseServices dbService)
{
    
    public async Task<string> GetStatisticHandler(HttpContext httpContext)
    {
        var stats = await dbService.GetUserTaskStatisticsAsync();
        return JsonSerializer.Serialize(stats);
    }
}