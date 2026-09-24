using GeoImport;

namespace Check;

public class Program
{
    public static void Main(string[] args)
    {
        var path = @"D:\Projects\upload\00000000-0000-0000-0000-000000000002";
        var service = new GeoImportService();
        var res = service.Parse(path);
        Console.WriteLine(res);
    }
}