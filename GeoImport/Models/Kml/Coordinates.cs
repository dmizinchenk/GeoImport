using System.ComponentModel;
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
            data = value;
        }
        get
        {
            var v = Values;
            if (v is null || v.Length == 0)
            {
                return null;
            }
            
            var builder = new StringBuilder(data.Length + v.Length * 3);
            
            for (var i = 0; i < data.Length; i++)
            {
                builder.Append($"[{data[i]}]");
                if (i != data.Length - 1)
                {
                    builder.Append(',');
                }
            }
            return builder.ToString();
        } 
    }
    [XmlIgnore]
    public string[] Values
    {
        get
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return data.Split([' ', '\n', '\t', '\r'], StringSplitOptions.RemoveEmptyEntries);
        }
    }
}