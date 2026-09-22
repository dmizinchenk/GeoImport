using System.ComponentModel;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
public class MultiGeometry : ICoordinates
{
    /// <remarks/>
    [XmlElement(typeof(Polygon), ElementName = "Polygon")]
    [XmlElement(typeof(LineString), ElementName = "LineString")]
    [XmlElement(typeof(LinearRing), ElementName = "LinearRing")]
    [XmlElement(typeof(Point), ElementName = "Point")]
    public object[] Geometries { get; set; }

    public string GetCoordinateString() => $"[{GetTypeGeom(Geometries).GetCoordinateString()}]";

    [XmlIgnore]
    public List<object> GetCoordinates
    {
        get
        {
            var type = GetTypeGeom(Geometries);
            return type == null ? [] : type.GetCoordinates;
        }
    }

    private ICoordinates GetTypeGeom(object[] objects)
    {
        if (objects == null || objects.Length == 0)
        {
            return null;
        }
        switch (objects[0])
        {
            case null:
                return null;
            case Polygon:
                return (Polygon)objects[0];
            case LineString:
                return (LineString)objects[0];
            case LinearRing:
                return (LinearRing)objects[0];
            case Point:
                return (Point)objects[0];
            default:
                throw new ArgumentException();
        }
    }
}