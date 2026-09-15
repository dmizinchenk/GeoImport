using System.ComponentModel;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
public class SchemaData
{
    /// <remarks/>
    [XmlElement("SimpleData")]
    public SimpleData[] SimpleData { get; set; }

    /// <remarks/>
    [XmlAttribute("schemaUrl")]
    public string SchemaUrl { get; set; }
}