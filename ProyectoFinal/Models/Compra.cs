namespace ProyectoFinal.Models;

public class Compra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }

    public Producto productos { get; set; } = new();

    public string Display => $"{productos.TipoBien} - {productos.Marca}";
}
