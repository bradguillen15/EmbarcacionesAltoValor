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

    public async Task<Compra?> GetCompraConSaldoAsync(long compraId)
    {
        try
        {
            var response = await _http.GetAsync(
                $"/rest/v1/vw_compra_con_saldo?Id=eq.{compraId}&select=Id,ClienteId,ProductoId,PrecioTotal,PrimaInicial,PlazoMeses,TasaInteres,Estado,FechaRegistro,SaldoPendiente,productos(Id,Nombre,TipoBien,Marca,PrecioBase)"
            );
            response.EnsureSuccessStatusCode();

            var compras = await response.Content.ReadFromJsonAsync<List<Compra>>();
            return compras?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo cargar el saldo: {ex.Message}", "OK");
            return null;
        }
    }

    public async Task<List<Compra>> GetComprasConSaldoByClienteAsync(long clienteId)
    {
        try
        {
            var response = await _http.GetAsync(
                $"/rest/v1/vw_compra_con_saldo?ClienteId=eq.{clienteId}&select=Id,ClienteId,ProductoId,PrecioTotal,PrimaInicial,PlazoMeses,TasaInteres,Estado,FechaRegistro,SaldoPendiente,productos(Id,Nombre,TipoBien,Marca,PrecioBase)&order=Estado.asc,FechaRegistro.desc"
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
