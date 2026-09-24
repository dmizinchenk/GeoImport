using System.Text.Json;
using GeoImport.Calc;
using GeoImport.Models.Json;
using GeoImport.Models.Kml;
using Style = GeoImport.Models.Json.Style;

namespace GeoImport;

public class KmlMapper : IMapper
{
    public string ConvertToJson(object? obj)
    {
        var jsonModel = new JsonRoot();
        if (obj == null)
        {
            return JsonSerializer.Serialize(jsonModel);;
        }
        var model = obj as Kml;
        var placeMarks = GetPlacemark(model?.Feature!);
        
        if (placeMarks == null || placeMarks.Length == 0)
        {
            return JsonSerializer.Serialize(jsonModel);;
        }
        
        foreach (var placemark in placeMarks)
        {
            var record = new object[5];
            record[0] = placemark.Name;
            record[1] = placemark.Description?.Value;
            record[2] = placemark.Style?.LineStyle?.Width;
            record[3] = placemark.Style?.PolyStyle?.Color;
            record[4] = placemark.Style?.LineStyle?.Color;
        
            var coordinates = GetCoordinates(placemark.Geometry, out var geometryType);
            
            double area = 0;
            double perimeter = 0;
            if (geometryType != typeof(Point))
            {
                var calculator = new GeodCalculator();
                (area, perimeter) = calculator.Calculate(coordinates);
            }
            var geometry = new Geometries
            {
                Area = area,
                Perimeter = perimeter,
                ShapeObj = new Shape
                {
                    Type = geometryType.Name,
                    Coordinates = ((ICoordinates)placemark.Geometry).GetCoordinates,
                },
                ShapeType = geometryType.Name,
                Style = new Style
                {
                    Color = placemark?.Style?.LineStyle?.Color,
                    Width = int.TryParse(placemark?.Style?.LineStyle?.Width, out var w) ? w : 0,
                },
            };
            
            jsonModel.Geometries.Add(geometry);
            jsonModel.Records.Add(record);
        }
        
        jsonModel.CountRecords = placeMarks.Length;
        jsonModel.Area = jsonModel.Geometries.Sum(g => g.Area);
        return JsonSerializer.Serialize(jsonModel);
    }

    private Placemark[]? GetPlacemark(object[] feature)
    {
        if (feature == null || feature.Length == 0)
        {
            return null;
        }
        
        return feature[0] switch
        {
            Document document => GetPlacemark(document.Feature),
            Folder folder => GetPlacemark(folder.Feature),
            Placemark => feature.Select(o => (Placemark)o).ToArray(),
            _ => throw new ArgumentException()
        };
    }

    private List<List<Coordinates>> GetCoordinates(object geometry, out Type geometryType)
    {
        List<List<Coordinates>> list;
        switch (geometry)
        {
            case null:
                geometryType = null;
                return null;
            case MultiGeometry multiGeometry:
                list = GetCoordinates(multiGeometry.Geometries, out _);
                geometryType = typeof(MultiGeometry);
                return list;
            case Polygon polygon:
                var outer = GetCoordinates(polygon.OuterBoundaryIs?.LinearRing, out _);
                var inner = GetCoordinates(polygon.InnerBoundaryIs?.LinearRing, out _);
                geometryType = typeof(Polygon);
                return inner == null ? [outer[0]] : [outer[0], inner[0]];
            case LineString lineString:
                geometryType = typeof(LineString);
                return [[lineString.Coordinates]];
            case LinearRing linearRing:
                geometryType = typeof(LinearRing);
                return [[linearRing.Coordinates]];
            case Point point:
                geometryType = typeof(Point);
                return [[point.Coordinates]];
            case object[] objects:
                return GetCoordinates(objects[0], out geometryType);
            default:
                throw new ArgumentException();
        }
    }
}