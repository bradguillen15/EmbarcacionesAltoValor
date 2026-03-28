namespace ProyectoFinal.Models;

public class LoginResponse
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

}

//metodo utilizado en compras para obtener el ID del cliente
public static class Session
{
    public static int ClienteId { get; set; }
}

