using System.ComponentModel;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
public class ExtendedData
{
    /// <remarks/>
    [XmlElement("SchemaData")]
    public SchemaData SchemaData { get; set; }
}