using System.ComponentModel;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
public class LineString
{
    /// <remarks/>
    [XmlElement("coordinates")]
    public Coordinates Coordinates { get; set; }
}