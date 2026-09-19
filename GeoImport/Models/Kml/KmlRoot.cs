using System.ComponentModel;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

// Примечание. Для запуска созданного кода может потребоваться NET Framework версии 4.5 или более поздней версии и .NET Core или Standard версии 2.0 или более поздней.
/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
[XmlRoot(ElementName = "kml", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
public class Kml
{
    /// <remarks/>
    [XmlElement(typeof(Document), ElementName = "Document")]
    [XmlElement(typeof(Folder), ElementName = "Folder")]
    [XmlElement(typeof(Placemark), ElementName = "Placemark")]
    public object Feature { get; set; }
}