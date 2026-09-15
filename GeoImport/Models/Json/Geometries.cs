using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeoImport.Models.Json;

public class Geometries
{
    public double Area { get; set; }
    public double Perimeter { get; set; }
    public string Shape => JsonSerializer.Serialize(ShapeObj);

    [JsonIgnore]
    public Shape ShapeObj { get; set; }
    public string ShapeType { get; set; }
    public Style Style { get; set; }
}