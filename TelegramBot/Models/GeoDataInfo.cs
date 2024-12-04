using System.Text.Json.Serialization;

namespace TelegramBot.Models;

public class GeoDataInfo
{
    [JsonPropertyName("longitude")] 
    public double Longitude { get; set; }

    [JsonPropertyName("latitude")] 
    public double Latitude { get; set; }

    [JsonPropertyName("location")] 
    public string Location { get; set; }

    [JsonPropertyName("iana_timezone")] 
    public string IanaTimezone { get; set; }

    [JsonPropertyName("timezone_abbreviation")]
    public string TimezoneAbbreviation { get; set; }

    [JsonPropertyName("offset")] 
    public string Offset { get; set; }

    public GeoDataInfo()
    {
        
    }
}