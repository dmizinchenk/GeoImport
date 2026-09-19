using System.ComponentModel;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
public class Placemark
{
    /// <remarks/>
    [XmlElement("name")]
    public string Name { get; set; }

    /// <remarks/>
    [XmlElement("Style")]
    public Style Style { get; set; }

    /// <remarks/>
    [XmlElement("ExtendedData")]
    public ExtendedData ExtendedData { get; set; }

    /// <remarks/>
    [XmlElement(typeof(MultiGeometry), ElementName = "MultiGeometry")]
    [XmlElement(typeof(Polygon), ElementName = "Polygon")]
    [XmlElement(typeof(LineString), ElementName = "LineString")]
    [XmlElement(typeof(LinearRing), ElementName = "LinearRing")]
    [XmlElement(typeof(Point), ElementName = "Point")]
    public object Geometry { get; set; }
    /// <remarks/>
    [XmlElement("description")]
    public Description Description { get; set; }
}