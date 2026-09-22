using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace GeoImport.Models.Kml;

/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
public class Coordinates
{
    private string data;
    /// <remarks/>
    [XmlText]
    public string Content {
        set
        {
            data = value.Trim();
        }
        get => data;
    }
    [XmlIgnore]
    public double[][] Values
    {
        get
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return data.Split([' ', '\n', '\t', '\r'], StringSplitOptions.RemoveEmptyEntries)
                .Select<string, double[]>(e =>
                {
                    var arr = e.Split(',');
                    return
                        [
                            double.Parse(arr[0], NumberStyles.Any, CultureInfo.InvariantCulture),
                            double.Parse(arr[1], NumberStyles.Any, CultureInfo.InvariantCulture),
                        ];
                }).ToArray();
        }
    }
}