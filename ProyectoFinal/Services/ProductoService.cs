using System.Net.Http.Json;
using ProyectoFinal.Config;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services;

public class ProductoService
{
    private readonly HttpClient _http;

    public ProductoService()
    {
        _http = new HttpClient();
        _http.BaseAddress = new Uri(AppConfig.SupabaseUrl);
        _http.DefaultRequestHeaders.Add("apikey", AppConfig.SupabaseAnonKey);
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {AppConfig.SupabaseAnonKey}");
    }

    public async Task<List<Producto>> GetProductosAsync()
    {
        try
        {
            var response = await _http.GetAsync("/rest/v1/productos?select=*");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Producto>>() ?? new List<Producto>();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudieron cargar los productos: {ex.Message}", "OK");
            return new List<Producto>();
        }
    }

    public async Task<Producto?> GetProductoByIdAsync(long id)
    {
        try
        {
            var response = await _http.GetAsync($"/rest/v1/productos?Id=eq.{id}&select=*");
            response.EnsureSuccessStatusCode();

            var productos = await response.Content.ReadFromJsonAsync<List<Producto>>();
            return productos?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo cargar el producto: {ex.Message}", "OK");
            return null;
        }
    }
}
