namespace GeoImport.Models.Json;

/// <remarks/>
public class JsonRoot
{
    /// <remarks/>
    public double Area { get; set; }
    /// <remarks/>
    public int CountRecords { get; set; }

    /// <remarks/>
    public string[] Fields { get; set; } =
    [
        "Name",
        "Description",
        "Width",
        "Fill",
        "Outline"
    ];

    /// <remarks/>
    public List<Geometries> Geometries { get; set; } = [];

    /// <remarks/>
    public List<object[]> Records { get; set; } = [];
}

