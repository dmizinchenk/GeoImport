namespace GeoImport.Models.Kml;

public interface ICoordinates
{
    string GetCoordinateString();
    List<object> GetCoordinates { get; }
}