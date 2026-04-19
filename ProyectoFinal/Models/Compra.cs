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

    [JsonPropertyName("TotalConIntereses")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TotalConIntereses { get; set; }

    [JsonIgnore]
    public string Display => Producto != null
        ? $"{Producto.TipoBien} - {Producto.Marca}"
        : $"Producto ID: {ProductoId}";

    [JsonIgnore]
    public decimal MontoFinanciado => PrecioTotal - PrimaInicial;

    [JsonIgnore]
    public decimal TasaMensual => TasaInteres > 0 ? TasaInteres / 12 / 100 : 0;

    [JsonIgnore]
    public decimal CuotaMensual
    {
        get
        {
            if (PlazoMeses <= 0) return 0;
            if (TasaMensual <= 0) return MontoFinanciado / PlazoMeses;
            return MontoFinanciado * TasaMensual /
                   (1 - (decimal)Math.Pow((double)(1 + TasaMensual), -PlazoMeses));
        }
    }

    [JsonIgnore]
    public decimal TotalRealAPagar => CuotaMensual * PlazoMeses;

    [JsonIgnore]
    public decimal TotalIntereses => TotalRealAPagar - MontoFinanciado;

    // Base de saldo: usa TotalConIntereses de BD si existe, sino calcula localmente
    [JsonIgnore]
    private decimal BaseDeuda => TotalConIntereses.HasValue && TotalConIntereses.Value > 0
        ? TotalConIntereses.Value
        : TotalRealAPagar;

    [JsonIgnore]
    public decimal TotalAbonado => BaseDeuda - (SaldoPendiente ?? BaseDeuda);

    [JsonIgnore]
    public double PorcentajePagado => BaseDeuda > 0
        ? (double)((TotalAbonado / BaseDeuda) * 100)
        : 0;
}
