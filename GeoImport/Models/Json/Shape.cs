using System.Text.Json.Serialization;

namespace GeoImport.Models.Json;

public class Shape
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("coordinates")] 
    public string Coordinates { get; set; }
}