using System.ComponentModel;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
public class Polygon : ICoordinates
{
    /// <remarks/>
    [XmlElement("outerBoundaryIs")]
    public OuterBoundaryIs OuterBoundaryIs { get; set; }
    /// <remarks/>
    [XmlElement("innerBoundaryIs")]
    public InnerBoundaryIs InnerBoundaryIs { get; set; }

    public string GetCoordinateString() =>
        $"[{OuterBoundaryIs.GetCoordinates()}{(InnerBoundaryIs == null ? "" : $",{InnerBoundaryIs.GetCoordinates()}")}]";

    [XmlIgnore]
    public List<object> GetCoordinates => InnerBoundaryIs == null ? [OuterBoundaryIs.Coordinates] : [OuterBoundaryIs.Coordinates, InnerBoundaryIs.Coordinates];
}