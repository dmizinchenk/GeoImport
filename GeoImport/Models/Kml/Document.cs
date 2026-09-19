using System.ComponentModel;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
public class Document
{
    /// <remarks/>
    [XmlElement("Schema")]
    public Schema? Schema { get; set; }

    /// <remarks/>
    [XmlElement(typeof(Folder), ElementName = "Folder")]
    [XmlElement(typeof(Placemark[]), ElementName = "Placemark")]
    public object Feature { get; set; }

    /// <remarks/>
    [XmlAttribute("id")]
    public string Id { get; set; }
}