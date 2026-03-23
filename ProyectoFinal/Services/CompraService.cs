using System.Net.Http.Json;
using ProyectoFinal.Config;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services;

public class CompraService
{
    private readonly HttpClient _http;

    public CompraService()
    {
        _http = new HttpClient();
        _http.BaseAddress = new Uri(AppConfig.SupabaseUrl);
        _http.DefaultRequestHeaders.Add("apikey", AppConfig.SupabaseAnonKey);
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {AppConfig.SupabaseAnonKey}");
    }


    public async Task<string> InsertAbono(Abono abono)
    {
        var response = await _http.PostAsJsonAsync("/rest/v1/abonos", abono);

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return content; //  devuelve error en caso de problemas
        }

        return "OK";
    }

    public async Task<List<Compra>> GetComprasByCliente(int clienteId)
    {
        var response = await _http.GetAsync(
        $"/rest/v1/compras?ClienteId=eq.{clienteId}&select=Id,ProductoId,productos(Nombre,TipoBien,Marca)"
    );
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Compra>>() ?? new List<Compra>();
    }
}
