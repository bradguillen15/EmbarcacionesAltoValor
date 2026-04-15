using System.Text.Json.Serialization;

namespace ProyectoFinal.Models;

public class Compra
{
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

    [JsonPropertyName("SaldoPendiente")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SaldoPendiente { get; set; }

    [JsonPropertyName("productos")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Producto? Producto { get; set; }

    [JsonIgnore]
    public string Display => Producto != null 
        ? $"{Producto.TipoBien} - {Producto.Marca}" 
        : $"Producto ID: {ProductoId}";

    [JsonIgnore]
    public decimal MontoFinanciado => PrecioTotal - PrimaInicial;

    [JsonIgnore]
    public decimal TotalAbonado => MontoFinanciado - (SaldoPendiente ?? MontoFinanciado);

    [JsonIgnore]
    public double PorcentajePagado => MontoFinanciado > 0 
        ? (double)((TotalAbonado / MontoFinanciado) * 100) 
        : 0;
}
