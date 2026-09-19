using System.ComponentModel;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
public class MultiGeometry
{
    /// <remarks/>
    [XmlElement(typeof(Polygon), ElementName = "Polygon")]
    [XmlElement(typeof(LineString[]), ElementName = "LineString")]
    [XmlElement(typeof(LinearRing[]), ElementName = "LinearRing")]
    [XmlElement(typeof(Point[]), ElementName = "Point")]
    public object Geometries { get; set; }
}