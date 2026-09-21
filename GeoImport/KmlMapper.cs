using System.Text.Json;
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
        
        // if (placeMarks == null || placeMarks.Length == 0)
        // {
        //     return JsonSerializer.Serialize(jsonModel);;
        // }
        //
        // foreach (var placemark in placeMarks)
        // {
        //     var record = new object[5];
        //     record[0] = placemark.Name;
        //     record[1] = placemark.Description?.Value;
        //     record[2] = placemark.Style.LineStyle?.Width;
        //     record[3] = placemark.Style.PolyStyle?.Color;
        //     record[4] = placemark.Style.LineStyle?.Color;
        //
        //     var coordinates = GetCoordinates(placemark.Geometry, out var geometryType);
        //     double area = 0;
        //     double perimeter = 0;
        //     if (geometryType is Point)
        //     {
        //         (area, perimeter) = CalcParams(coordinates);
        //     }
        //     var geometry = new Geometries
        //     {
        //         Area = area,
        //         Perimeter = perimeter,
        //         ShapeObj = new Shape
        //         {
        //             Type = geometryType.Name,
        //             Coordinates = null
        //         },
        //         ShapeType = geometryType.Name,
        //         Style = new Style
        //         {
        //             Color = placemark?.Style?.LineStyle?.Color,
        //             Width = int.TryParse(placemark?.Style?.LineStyle?.Width, out var w) ? w : 0,
        //         },
        //     };
        //     
        //     jsonModel.Geometries.Add(geometry);
        //     jsonModel.Records.Add(record);
        // }
        
        // jsonModel.CountRecords = placeMarks.Length;
        jsonModel.Area = jsonModel.Geometries.Sum(g => g.Area);
        return JsonSerializer.Serialize(jsonModel);
    }

    private Placemark[]? GetPlacemark(object[] feature) => 
        feature switch
        {
            null => null,
            var arr => arr[0] switch
            {
                Document document => GetPlacemark(document.Feature),
                Folder folder => GetPlacemark(folder.Feature),
                Placemark => arr.Select(o => (Placemark)o).ToArray(),
                _ => throw new ArgumentException()
            }
        };

    private List<List<Coordinates>> GetCoordinates(object geometry, out Type geometryType)
    {
        List<List<Coordinates>> list;
        switch (geometry)
        {
            case MultiGeometry multiGeometry:
                list = GetCoordinates(multiGeometry.Geometries, out _);
                geometryType = typeof(MultiGeometry);
                return list;
            case Polygon polygon:
                var outer = GetCoordinates(polygon.OuterBoundaryIs?.LinearRing, out _);
                var inner = GetCoordinates(polygon.InnerBoundaryIs?.LinearRing, out _);
                geometryType = typeof(Polygon);
                return [outer[0], inner[0]];
            case LineString lineString:
                geometryType = typeof(LineString);
                return [[lineString.Coordinates]];
            case LinearRing linearRing:
                geometryType = typeof(LinearRing);
                return [[linearRing.Coordinates]];
            case Point point:
                geometryType = typeof(Point);
                return [[point.Coordinates]];
            case Polygon[] polygons:
                list = new List<List<Coordinates>>(polygons.Select(polygon => GetCoordinates(polygon, out _)[0]));
                geometryType = typeof(Polygon);
                return list;
            case LineString[] lineStrings:
                list = new List<List<Coordinates>>(lineStrings.Select(lineString => GetCoordinates(lineString, out _)[0]));
                geometryType = typeof(LineString);
                return list;
            case LinearRing[] linearRings:
                list = new List<List<Coordinates>>(linearRings.Select(linearRing => GetCoordinates(linearRing, out _)[0]));
                geometryType = typeof(LinearRing);
                return list;
            case Point[] points:
                list = new List<List<Coordinates>>(points.Select(point => GetCoordinates(point, out _)[0]));
                geometryType = typeof(Point);
                return list;
            default:
                throw new ArgumentException();
        }
    }

    private (double area, double perimeter) CalcParams(List<List<Coordinates>> coordinates)
    {
        return (10, 10);
        //TODO
        foreach (var coordinate in coordinates)
        {
        }
    }
    // {
    //     if (obj == null)
    //     {
    //         return "";
    //     }
    //     
    //     var model = obj as Kml;
    //
    //     var jsonModel = new JsonRoot();
    //
    //     var multiGeometries = model?.Document?.Folder?.Placemark?.MultiGeometry;
    //
    //     if (multiGeometries is null)
    //     {
    //         return "";
    //     }
    //
    //
    //     var counter = 0;
    //     foreach (var multiGeometry in multiGeometries)
    //     {
    //         var polygonElements = multiGeometry.Polygon;
    //         if (polygonElements is null)
    //         {
    //             return "";
    //         }
    //         var polygons = new List<Polygon>();
    //         var calculator = new GeodCalculator();
    //         var factory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
    //
    //         var record = new object[5];
    //         foreach (var polygonElement in polygonElements)
    //         {
    //             var outerCoordinates = polygonElement.OuterBoundaryIs?.LinearRing?.Coordinates?
    //                 .Split([' ', '\n', '\t', '\r'], StringSplitOptions.RemoveEmptyEntries);
    //
    //             if (outerCoordinates == null)
    //             {
    //                 return "";
    //             }
    //
    //             var shellCoords = ParseCoordinates(outerCoordinates);
    //             var shell = factory.CreateLinearRing(shellCoords.ToArray());
    //
    //             var holes = new List<LinearRing>();
    //             var innerBoundaries = polygonElement.InnerBoundaryIs;
    //             if (innerBoundaries is not null)
    //             {
    //                 foreach (var innerBoundary in innerBoundaries)
    //                 {
    //                     var innerCoordinates = innerBoundary.LinearRing?.Coordinates?
    //                         .Split([' ', '\n', '\t', '\r'], StringSplitOptions.RemoveEmptyEntries);
    //                     if (innerCoordinates == null)
    //                     {
    //                         continue;
    //                     }
    //
    //                     var holeCoords = ParseCoordinates(innerCoordinates);
    //                     holes.Add(factory.CreateLinearRing(holeCoords.ToArray()));
    //                 }
    //             }
    //
    //             var polygon = factory.CreatePolygon(shell, holes.ToArray());
    //             polygons.Add(polygon);
    //
    //         }
    //
    //         record[0] = model?.Document?.Folder?.Placemark?.Name;
    //         record[1] = model?.Document?.Folder?.Placemark?.Description?.Value;
    //         record[2] = model?.Document?.Folder?.Placemark?.Style.LineStyle?.Width;
    //         record[3] = model?.Document?.Folder?.Placemark?.Style.PolyStyle?.Color;
    //         record[4] = model?.Document?.Folder?.Placemark?.Style.LineStyle?.Color;
    //         jsonModel.Records.Add(record);
    //
    //         var multiPolygon = factory.CreateMultiPolygon(polygons.ToArray());
    //         var characters = calculator.GeometryAreaPerimeter(multiPolygon);
    //         var geometry = new Geometries
    //         {
    //             Area = characters.Area,
    //             Perimeter = characters.Perimeter,
    //             ShapeObj = new Shape
    //             {
    //                 Type = multiPolygon.GeometryType,
    //                 Coordinates = ToText(multiPolygon),
    //             },
    //             ShapeType = multiPolygon.GeometryType,
    //             Style = new Style
    //             {
    //                 Color = model?.Document?.Folder?.Placemark?.Style.LineStyle?.Color,
    //                 Width = 0
    //             },
    //         };
    //         jsonModel.Geometries.Add(geometry);
    //         counter++;
    //     }
    //     
    //     jsonModel.CountRecords = counter;
    //     jsonModel.Area = jsonModel.Geometries.Sum(g => g.Area);
    //     return JsonSerializer.Serialize(jsonModel);
    // }
    //
    // private List<Coordinate> ParseCoordinates(string[] coords)
    // {
    //     var result = new List<Coordinate>(coords.Length);
    //     foreach (var point in coords)
    //     {
    //         var coordinate = point.Split(',', StringSplitOptions.RemoveEmptyEntries);
    //         if (coordinate.Length >= 2 &&
    //             double.TryParse(coordinate[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var x) &&
    //             double.TryParse(coordinate[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var y))
    //         {
    //             result.Add(new Coordinate(x, y));
    //         }
    //     }
    //     return result;
    // }

    // private string ToText(Geometry geometry)
    // {
    //     var startString = geometry.ToString();
    //     var points = startString[(startString.IndexOf(' ') + 1)..].Replace('(', '[').Replace(')', ']')
    //         .Split(", ");
    //     return string.Join(',', points.Select(p => $"[{p.Replace(' ', ',')}]").ToArray());
    // }
}