namespace NuxibaAccesos.Api.Models;

// Catálogo de áreas. Tabla ccRIACat_Areas.
public class Area
{
    public int IDArea { get; set; }

    public string AreaName { get; set; } = string.Empty;

    public int StatusArea { get; set; }

    public DateTime CreateDate { get; set; }
}
