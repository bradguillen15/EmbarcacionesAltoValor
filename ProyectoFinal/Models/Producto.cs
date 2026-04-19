using System.Text.Json.Serialization;

namespace ProyectoFinal.Models;

public class Producto
{
    /// <summary>
    /// ID del producto. Es auto-generado por Supabase.
    /// </summary>
    [JsonPropertyName("Id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long Id { get; set; }

    [JsonPropertyName("ProveedorId")]
    public long ProveedorId { get; set; }

    [JsonPropertyName("Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("TipoBien")]
    public string TipoBien { get; set; } = string.Empty;

    [JsonPropertyName("Marca")]
    public string Marca { get; set; } = string.Empty;

    [JsonPropertyName("Capacidad")]
    public int? Capacidad { get; set; }

    [JsonPropertyName("Zona")]
    public string? Zona { get; set; }

    [JsonPropertyName("Caracteristicas")]
    public string? Caracteristicas { get; set; }

    [JsonPropertyName("FotoUrl")]
    public string? FotoUrl { get; set; }

    [JsonPropertyName("FichaUrl")]
    public string? FichaUrl { get; set; }

    [JsonPropertyName("PrecioBase")]
    public decimal? PrecioBase { get; set; }

    [JsonIgnore]
    public string DisplayText => $"{TipoBien} {Marca} - {Nombre}";

    [JsonIgnore]
    public string CapacidadText => Capacidad.HasValue ? $"{Capacidad} personas" : "No especificado";

    [JsonIgnore]
    public string PrecioFormateado => PrecioBase.HasValue ? $"₡{PrecioBase.Value:N2}" : "Precio no disponible";
}
