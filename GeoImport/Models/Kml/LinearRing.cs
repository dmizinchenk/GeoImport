using System.ComponentModel;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
public class LinearRing : ICoordinates
{
    /// <remarks/>
    [XmlElement("coordinates")]
    public Coordinates Coordinates { get; set; }

    public string GetCoordinateString() => $"[{Coordinates?.Content}]";
    
    [XmlIgnore]
    public List<object> GetCoordinates => [..Coordinates.Values.Cast<object>()];
}