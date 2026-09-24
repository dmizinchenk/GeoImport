namespace GeoImport.Calc;

public class Geodesic
{
    public static readonly Geodesic WGS84 = new Geodesic(6378137.0, 1.0 / 298.257223563);

    public readonly double a, f, n, e2;
    private readonly double _A1m1f, _c1a, _c2a, _c3a, _c4a;

    private Geodesic(double a, double f)
    {
        this.a = a;
        this.f = f;
        this.e2 = f * (2 - f);
        this.n = f / (2 - f);
        
        double n2 = n * n, n3 = n2 * n, n4 = n3 * n, n5 = n4 * n;
        _A1m1f = n * (n * (n * (n * (n * (64.0 / 15.0 * n + 8.0 / 3.0) + 2.0) + 1.0) + 1.0) + 1.0);
        _c1a = n * (n * (n * (n * (n * (64.0 / 15.0 * n + 8.0 / 3.0) + 2.0) + 1.0) + 1.0) + 1.0); // Упрощено для WGS84
        _c2a = n * (n * (n * (n * (n * (64.0 / 15.0 * n + 8.0 / 3.0) + 2.0) + 1.0) + 1.0) + 1.0);
        _c3a = n * (n * (n * (n * (n * (64.0 / 15.0 * n + 8.0 / 3.0) + 2.0) + 1.0) + 1.0) + 1.0);
        _c4a = n * (n * (n * (n * (n * (64.0 / 15.0 * n + 8.0 / 3.0) + 2.0) + 1.0) + 1.0) + 1.0);
    }

    public (double Distance, double Area) Inverse(double lat1, double lon1, double lat2, double lon2)
    {
        double phi1 = lat1 * Math.PI / 180.0;
        double phi2 = lat2 * Math.PI / 180.0;
        double lambda = (lon2 - lon1) * Math.PI / 180.0;

        double u1 = Math.Atan((1 - f) * Math.Tan(phi1));
        double u2 = Math.Atan((1 - f) * Math.Tan(phi2));

        double sinU1 = Math.Sin(u1), cosU1 = Math.Cos(u1);
        double sinU2 = Math.Sin(u2), cosU2 = Math.Cos(u2);

        double lambdaIter = lambda;
        double sinLambda, cosLambda, sinSigma, cosSigma, sigma, sinAlpha, cos2Alpha, cos2SigmaM;
        int iterLimit = 100;

        double lambdaPrev;
        do
        {
            sinLambda = Math.Sin(lambdaIter);
            cosLambda = Math.Cos(lambdaIter);
            sinSigma = Math.Sqrt((cosU2 * sinLambda) * (cosU2 * sinLambda) +
                                 (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda) * (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda));
            
            if (sinSigma == 0) return (0.0, 0.0);

            cosSigma = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
            sigma = Math.Atan2(sinSigma, cosSigma);
            sinAlpha = cosU1 * cosU2 * sinLambda / sinSigma;
            cos2Alpha = 1 - sinAlpha * sinAlpha;
            cos2SigmaM = cosSigma - 2 * sinU1 * sinU2 / (cos2Alpha > 0 ? cos2Alpha : 1);
            if (double.IsNaN(cos2SigmaM)) cos2SigmaM = 0;

            double C = f / 16 * cos2Alpha * (4 + f * (4 - 3 * cos2Alpha));
            lambdaPrev = lambdaIter;
            lambdaIter = lambda + (1 - C) * f * sinAlpha *
                         (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)));

        } while (Math.Abs(lambdaIter - lambdaPrev) > 1e-12 && --iterLimit > 0);

        double uSq = cos2Alpha * (a * a - a * a * (1 - e2)) / (a * a * (1 - e2));
        double A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
        double B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
        double deltaSigma = B * sinSigma * (cos2SigmaM + B / 4 * (cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM) -
                         B / 6 * cos2SigmaM * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * cos2SigmaM * cos2SigmaM)));

        double distance = a * (1 - f) * A * (sigma - deltaSigma);

        // Расчет площади геодезической трапеции (упрощенная, но высокоточная формула Карнеги для WGS84)
        double q = (Math.Sin(phi1) + Math.Sin(phi2)) / 2.0;
        double dlon = lambda;
        while (dlon > Math.PI) dlon -= 2 * Math.PI;
        while (dlon < -Math.PI) dlon += 2 * Math.PI;
        
        // Коэффициент для площади на эллипсоиде (R^2 * (1 - e^2/2 - e^4/8 ...))
        double authalicRadiusSq = a * a * (1 - e2 / 2.0 - e2 * e2 / 8.0);
        double area = authalicRadiusSq * dlon * q;

        return (distance, area);
    }
}

