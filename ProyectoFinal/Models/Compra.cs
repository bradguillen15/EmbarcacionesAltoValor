using System.Text.Json.Serialization;

namespace ProyectoFinal.Models;

public class Compra
{
    [JsonPropertyName("Id")]
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

    [JsonIgnore]
    public Producto? productos { get; set; }

    [JsonIgnore]
    public string Display => productos != null 
        ? $"{productos.TipoBien} - {productos.Marca}" 
        : $"Producto ID: {ProductoId}";

    [JsonIgnore]
    public decimal MontoFinanciado => PrecioTotal - PrimaInicial;
}
