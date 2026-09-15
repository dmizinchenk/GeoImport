using System.ComponentModel;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
public class Style
{
    /// <remarks/>
    [XmlElement("LineStyle")]
    public LineStyle LineStyle { get; set; }

    /// <remarks/>
    [XmlElement("PolyStyle")]
    public PolyStyle PolyStyle { get; set; }
}