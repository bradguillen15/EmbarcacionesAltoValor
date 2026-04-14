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
        _http.DefaultRequestHeaders.Add("Prefer", "return=representation");
    }

    public async Task<bool> RegistrarCompraAsync(Compra compra)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("/rest/v1/compras", compra);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo registrar la compra: {error}", "OK");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Error al registrar compra: {ex.Message}", "OK");
            return false;
        }
    }

    public async Task<List<Compra>> GetComprasByClienteAsync(long clienteId)
    {
        try
        {
            var response = await _http.GetAsync(
                $"/rest/v1/compras?ClienteId=eq.{clienteId}&select=Id,ClienteId,ProductoId,PrecioTotal,PrimaInicial,PlazoMeses,TasaInteres,Estado,FechaRegistro,productos(Id,Nombre,TipoBien,Marca,PrecioBase)"
            );
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Compra>>() ?? new List<Compra>();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudieron cargar las compras: {ex.Message}", "OK");
            return new List<Compra>();
        }
    }
}
