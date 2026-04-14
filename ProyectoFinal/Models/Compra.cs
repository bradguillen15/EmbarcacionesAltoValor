using System.Text.Json.Serialization;

namespace ProyectoFinal.Models;

public class Compra
{
    /// <summary>
    /// ID de la compra. Es auto-generado por Supabase, no enviar en POST.
    /// </summary>
    [JsonPropertyName("Id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long Id { get; set; }

    [JsonPropertyName("ClienteId")]
    public long ClienteId { get; set; }

    [JsonPropertyName("ProductoId")]
    public long ProductoId { get; set; }

    [JsonPropertyName("PrecioTotal")]
    public decimal PrecioTotal { get; set; }

    [JsonPropertyName("PrimaInicial")]
    public decimal PrimaInicial { get; set; }

    [JsonPropertyName("PlazoMeses")]
    public int PlazoMeses { get; set; }

    [JsonPropertyName("TasaInteres")]
    public decimal TasaInteres { get; set; }

    [JsonPropertyName("Estado")]
    public string Estado { get; set; } = "activo";

    [JsonPropertyName("FechaRegistro")]
    public DateTime FechaRegistro { get; set; }

    /// <summary>
    /// Propiedad de navegación para el producto asociado a esta compra.
    /// Se llena cuando se hace un select anidado en Supabase (productos).
    /// </summary>
    [JsonPropertyName("productos")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Producto? Producto { get; set; }

    [JsonIgnore]
    public string Display => Producto != null 
        ? $"{Producto.TipoBien} - {Producto.Marca}" 
        : $"Producto ID: {ProductoId}";

    [JsonIgnore]
    public decimal MontoFinanciado => PrecioTotal - PrimaInicial;
}
