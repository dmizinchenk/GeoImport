using System.Globalization;
using System.Text.Json;
using GeoImport.Models.Json;
using GeoImport.Models.Kml;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using LinearRing = NetTopologySuite.Geometries.LinearRing;
using Polygon = NetTopologySuite.Geometries.Polygon;
using Style = GeoImport.Models.Json.Style;

namespace GeoImport;

public class KmlMapper : IMapper
{
    public string ConvertToJson(object? obj)
    {
        if (obj == null)
        {
            return "";
        }
        
        var model = obj as Kml;
        var records = new List<object[]>();

        var jsonModel = new JsonRoot();

        var multiGeometry = model?.Document?.Folder?.Placemark?.MultiGeometry;

        if (multiGeometry is null)
        {
            return "";
        }
        
        var polygons = new List<Polygon>();
        var polygonElements = multiGeometry.Polygon;
        if (polygonElements is null)
        {
            return "";
        }

        var counter = 0;
        var perimeter = 0d;
        var factory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        foreach (var polygonElement in polygonElements)
        {
            records.Add(new object[5]);
            var outerCoordinates = polygonElement.OuterBoundaryIs?.LinearRing?.Coordinates?
                .Split([' ', '\n', '\t', '\r'], StringSplitOptions.RemoveEmptyEntries);

            if (outerCoordinates == null)
            {
                return "";
            }

            var shellCoords = ParseCoordinates(outerCoordinates);
            var shell = factory.CreateLinearRing(shellCoords.ToArray());

            var holes = new List<LinearRing>();
            var innerBoundaries = polygonElement.InnerBoundaryIs;
            if (innerBoundaries is not null)
            {
                foreach (var innerBoundary in innerBoundaries)
                {
                    var innerCoordinates = innerBoundary.LinearRing?.Coordinates?
                        .Split([' ', '\n', '\t', '\r'], StringSplitOptions.RemoveEmptyEntries);
                    if (innerCoordinates == null)
                    {
                        continue;
                    }
                    var holeCoords = ParseCoordinates(innerCoordinates);
                    holes.Add(factory.CreateLinearRing(holeCoords.ToArray()));
                }
            }
            var polygon = factory.CreatePolygon(shell, holes.ToArray());
            polygons.Add(polygon);
            perimeter += polygon.Length;
            
            records[counter][0] = model?.Document?.Folder?.Placemark?.Name;
            records[counter][1] = model?.Document?.Folder?.Placemark?.Description?.Value;
            records[counter][2] = model?.Document?.Folder?.Placemark?.Style.LineStyle?.Width;
            records[counter][3] = model?.Document?.Folder?.Placemark?.Style.PolyStyle?.Color;
            records[counter][4] = model?.Document?.Folder?.Placemark?.Style.LineStyle?.Color;
            counter++;
        }

        var multiPolygon = factory.CreateMultiPolygon(polygons.ToArray());
        var geometry = new Geometries
        {
            Area = multiPolygon.Area,
            Perimeter = perimeter,
            ShapeObj = new Shape
            {
                Type = multiPolygon.GeometryType,
                Coordinates = ToText(multiPolygon),
            },
            ShapeType = multiPolygon.GeometryType,
            Style = new Style
            {
                Color = model?.Document?.Folder?.Placemark?.Style.LineStyle?.Color,
                Width = 0
            },
        };

        jsonModel.Geometries.Add(geometry);
        jsonModel.CountRecords = counter;
        jsonModel.Area = jsonModel.Geometries.Sum(g => g.Area);
        jsonModel.Records = records;
        return JsonSerializer.Serialize(jsonModel);
    }

    private List<Coordinate> ParseCoordinates(string[] coords)
    {
        var result = new List<Coordinate>(coords.Length);
        foreach (var point in coords)
        {
            var coordinate = point.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (coordinate.Length >= 2 &&
                double.TryParse(coordinate[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var x) &&
                double.TryParse(coordinate[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var y))
            {
                result.Add(new Coordinate(x, y));
            }
        }
        return result;
    }

    private string ToText(Geometry geometry)
    {
        var startString = geometry.ToString();
        var points = startString[(startString.IndexOf(' ') + 1)..].Replace('(', '[').Replace(')', ']')
            .Split(", ");
        return string.Join(',', points.Select(p => $"[{p.Replace(' ', ',')}]").ToArray());
    }
}