using System.ComponentModel;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
public class Schema
{

    /// <remarks/>
    [XmlElement("SimpleField")]
    public SimpleField[] SimpleField { get; set; }

    /// <remarks/>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <remarks/>
    [XmlAttribute("id")]
    public string Id { get; set; }
}