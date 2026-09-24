namespace GeoImport.Calc;

public class PolygonArea
{
    private readonly Geodesic _earth;
    private readonly bool _polyline;
    private int _numPoints;
    private double _areaSum;
    private double _perimeter;
    private double _lat1, _lon1;
    private double _lat0, _lon0;

    public PolygonArea(Geodesic earth, bool polyline)
    {
        _earth = earth;
        _polyline = polyline;
        _numPoints = 0;
        _areaSum = 0;
        _perimeter = 0;
    }

    public void AddPoint(double lat, double lon)
    {
        if (_numPoints == 0)
        {
            _lat0 = _lat1 = lat;
            _lon0 = _lon1 = lon;
        }
        else
        {
            var result = _earth.Inverse(_lat1, _lon1, lat, lon);
            _perimeter += result.Distance;
            
            if (!_polyline)
            {
                _areaSum += result.Area;
            }
        }
        _lat1 = lat;
        _lon1 = lon;
        _numPoints++;
    }

    public (double Area, double Perimeter) Compute()
    {
        if (_numPoints < 2) return (0.0, 0.0);

        double perimeter = _perimeter;
        double area = _areaSum;

        if (!_polyline && _numPoints > 2)
        {
            // Замыкаем полигон
            var result = _earth.Inverse(_lat1, _lon1, _lat0, _lon0);
            perimeter += result.Distance;
            area += result.Area;

            // Нормализация площади по алгоритму Карнеги (GeographicLib)
            // Определяем, находится ли полигон в основном в северном или южном полушарии
            double hemisphereArea = Math.PI * _earth.a * _earth.a * (1 - _earth.e2 / 2.0 - _earth.e2 * _earth.e2 / 8.0);
            
            // Если площадь положительная и больше половины сферы, или отрицательная и меньше половины, корректируем
            if (area > 0)
            {
                if (area > hemisphereArea) area -= 2 * hemisphereArea;
            }
            else
            {
                if (area < -hemisphereArea) area += 2 * hemisphereArea;
            }
        }
        else if (_polyline)
        {
            area = 0.0;
        }

        return (area, perimeter);
    }
}
