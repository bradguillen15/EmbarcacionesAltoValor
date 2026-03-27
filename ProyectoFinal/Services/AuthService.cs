using System.Net.Http.Json;
using ProyectoFinal.Config;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services;

public class AuthService
{
    private readonly HttpClient _http;

    public AuthService()
    {
        _http = new HttpClient();
        _http.BaseAddress = new Uri(AppConfig.SupabaseUrl);
        _http.DefaultRequestHeaders.Add("apikey", AppConfig.SupabaseAnonKey);
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {AppConfig.SupabaseAnonKey}");
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var body = new { p_email = email, p_password = password };
        var response = await _http.PostAsJsonAsync("/rest/v1/rpc/login", body);
        response.EnsureSuccessStatusCode();

        var results = await response.Content.ReadFromJsonAsync<List<LoginResponse>>();
        return results?.FirstOrDefault();
    }

    //Task para insertar abono
    public async Task<string> InsertAbono(Abono abono)
    {
        var response = await _http.PostAsJsonAsync("/rest/v1/abonos", abono);

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return content; //  devuelve error en caso de problemas
        }

        return content;
    }
    //Task para obtener las compras del cliente seleccionado
    public async Task<List<Compra>> GetComprasByCliente(int clienteId)
    {
        var response = await _http.GetAsync(
        $"/rest/v1/compras?ClienteId=eq.{clienteId}&select=Id,ProductoId,productos(Nombre,TipoBien,Marca)"
    );
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Compra>>() ?? new List<Compra>();
    }
}