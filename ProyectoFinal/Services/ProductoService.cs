using System.Net.Http.Json;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services;

public class ProductoService
{
    private readonly HttpClient _http = SupabaseHttpClient.Instance;

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
