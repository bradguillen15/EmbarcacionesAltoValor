using System.Net.Http.Json;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services;

public class AbonoService
{
    private readonly HttpClient _http = SupabaseHttpClient.Instance;

    public async Task<bool> InsertAbonoAsync(Abono abono)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("/rest/v1/abonos", abono);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo registrar el abono: {error}", "OK");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Error al registrar abono: {ex.Message}", "OK");
            return false;
        }
    }

    public async Task<List<Abono>> GetAbonosByCompraAsync(long compraId)
    {
        try
        {
            var response = await _http.GetAsync($"/rest/v1/abonos?CompraId=eq.{compraId}&select=*&order=FechaAbono.desc");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Abono>>() ?? new List<Abono>();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudieron cargar los abonos: {ex.Message}", "OK");
            return new List<Abono>();
        }
    }

    // Obtiene todos los abonos del cliente consolidados desde todas sus compras
    public async Task<List<(Abono Abono, Compra Compra)>> GetAllAbonosByClienteAsync(int clienteId)
    {
        var compraService = new CompraService();
        var compras = await compraService.GetComprasConSaldoByClienteAsync(clienteId);
        var resultado = new List<(Abono, Compra)>();

        foreach (var compra in compras ?? new List<Compra>())
        {
            var abonos = await GetAbonosByCompraAsync(compra.Id);
            foreach (var abono in abonos)
                resultado.Add((abono, compra));
        }

        // Ordenar todos por fecha descendente
        return resultado.OrderByDescending(x => x.Item1.FechaAbono).ToList();
    }
}
