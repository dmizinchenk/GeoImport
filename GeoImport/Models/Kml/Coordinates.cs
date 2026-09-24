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
    public decimal[][] Values
    {
        get
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return data.Split([' ', '\n', '\t', '\r'], StringSplitOptions.RemoveEmptyEntries)
                .Select<string, decimal[]>(e =>
                {
                    var arr = e.Split(',');
                    return
                        [
                            decimal.Parse(arr[0], NumberStyles.Any, CultureInfo.InvariantCulture),
                            decimal.Parse(arr[1], NumberStyles.Any, CultureInfo.InvariantCulture),
                        ];
                }).ToArray();
        }
    }
}