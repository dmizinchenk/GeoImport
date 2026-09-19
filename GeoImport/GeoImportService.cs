using System.Xml;
using System.Xml.Serialization;
using GeoImport.Models.Kml;

namespace GeoImport;

public class GeoImportService
{
    /// <summary>
    /// Парсит файл в Json
    /// </summary>
    /// <param name="fullPath">Полный путь до папки, содержащей файл</param>
    /// <returns></returns>
    public string Parse(string fullPath)
    {
        if (!Directory.Exists(fullPath))
        {
            return "";
        }

        var files = Directory.GetFiles(fullPath);

        if (files.Length == 0)
        {
            return "";
        }

        switch (Path.GetExtension(files[0]).ToLower())
        {
            case ".kml":
                return Parser(files[0], typeof(Kml));
        }
        return "";
    }

    private string Parser(string fileName, Type type)
    {
        var settings = new XmlReaderSettings
        {
            Async = true
        };

        using var reader = XmlReader.Create(fileName, settings);
        var serializer = new XmlSerializer(type);
        var obj = serializer.Deserialize(reader);
        
        var mapper = new KmlMapper();
        return mapper.ConvertToJson(obj);
    }
}