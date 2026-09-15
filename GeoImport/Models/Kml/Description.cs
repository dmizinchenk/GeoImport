using System.ComponentModel;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
public class Description
{
    /// <remarks/>
    [XmlAttribute("color")]
    public string Color { get; set; }
    /// <remarks/>
    [XmlAttribute("img_data")]
    public string Img { get; set; }
    /// <remarks/>
    [XmlAttribute("width")]
    public string Width { get; set; }
    /// <remarks/>
    [XmlText]
    public string Value { get; set; }
}