using System.Net.Http.Json;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services;

public class CompraService
{
    private readonly HttpClient _http = SupabaseHttpClient.Instance;

    public async Task<bool> RegistrarCompraAsync(Compra compra)
    {
        try
        {
            // Agregar el header "Prefer" solo para esta petición
            using var request = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/compras");
            request.Headers.Add("Prefer", "return=representation");
            request.Content = JsonContent.Create(compra);

            var response = await _http.SendAsync(request);

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
