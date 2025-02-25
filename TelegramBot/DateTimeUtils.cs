﻿using System.Globalization;
using System.Text.Json;
using Humanizer;
using TelegramBot.Models;

namespace TelegramBot;

public class DateTimeUtils
{
    public static async Task<TimeZoneInfo> GetTimeZoneByGeoLocation(double latitude, double longitude)
    {
        var uri = string.Format(CultureInfo.InvariantCulture,
            "https://api.geotimezone.com/public/timezone?latitude={0}&longitude={1}", latitude, longitude);
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var geoDataInfo = JsonSerializer.Deserialize<GeoDataInfo>(responseBody);
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(geoDataInfo.IanaTimezone);
        }
        catch
        {
            return TimeZoneInfo.Local;
        }
    }

    public static string GetPresentInterval(int interval)
    {
        return interval.Minutes().Humanize();
    }

    public static TimeZoneInfo? GetTimeZoneByOffset(int offset)
    {
        return TimeZoneInfo.GetSystemTimeZones() .FirstOrDefault(tz => tz.BaseUtcOffset == TimeSpan.FromHours(offset));
    }
    
}