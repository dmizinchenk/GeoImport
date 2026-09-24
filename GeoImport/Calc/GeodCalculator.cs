using GeoImport.Models.Kml;

namespace GeoImport.Calc;
public class GeodCalculator
{
    private static readonly Geodesic GeodWgs84 = Geodesic.WGS84;

    /// <summary>
    /// Рассчитывает площадь (в гектарах) и периметр (в метрах).
    /// Полностью повторяет логику pyproj.Geod.geometry_area_perimeter + abs(area)/10000
    /// </summary>
    public (double AreaHa, double PerimeterM) Calculate(List<List<Coordinates>> geometries)
    {
        double totalArea = 0.0;
        double totalPerimeter = 0.0;

        if (geometries == null) return (0.0, 0.0);

        foreach (var geometry in geometries)
        {
            if (geometry == null || geometry.Count == 0) continue;

            // 1. Обрабатываем внешний контур (Exterior)
            var extResult = ProcessRing(geometry[0]);
            totalArea += extResult.Area;
            totalPerimeter += extResult.Perimeter; // Периметр считается только по внешнему контуру, как в pyproj

            // 2. Обрабатываем внутренние кольца (Holes/Interiors)
            // В GeographicLib/pyproj площадь дырок возвращается отрицательной (при правильной ориентации CW).
            // Поэтому мы просто СКЛАДЫВАЕМ их, и они математически вычтутся из общей площади.
            for (int i = 1; i < geometry.Count; i++)
            {
                var holeResult = ProcessRing(geometry[i]);
                totalArea += holeResult.Area;
            }
        }

        // В вашем Python коде: geo["Area"] = abs(poly_area) / 10_000
        // Мы применяем Math.Abs к итоговой "чистой" площади полигона (с учетом вычтенных дырок)
        return (Math.Abs(totalArea) / 10_000.0, totalPerimeter);
    }

    private (double Area, double Perimeter) ProcessRing(Coordinates coords)
    {
        if (coords == null || coords.Values == null || coords.Values.Length < 2)
            return (0.0, 0.0);

        // false означает, что мы не требуем явного замыкания первой и последней точкой 
        // (GeographicLib сам корректно замкнет полигон при Compute)
        var polyArea = new PolygonArea(GeodWgs84, false);

        foreach (var point in coords.Values)
        {
            if (point == null || point.Length < 2) continue;
            
            double lon = (double)point[0];
            double lat = (double)point[1];
            
            // В GeographicLib порядок аргументов строго: широта (lat), затем долгота (lon)
            polyArea.AddPoint(lat, lon);
        }

        var result = polyArea.Compute();
        return (result.Area, result.Perimeter);
    }
}
