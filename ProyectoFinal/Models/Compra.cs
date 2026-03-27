namespace ProyectoFinal.Models;

public class Compra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }

    public Producto producto { get; set; } = new();

    public string Display => $"{producto.TipoBien} - {producto.Marca}";
}
