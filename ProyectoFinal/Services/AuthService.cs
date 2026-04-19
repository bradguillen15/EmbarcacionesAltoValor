using System.Net.Http.Json;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services;

public class AuthService
{
    private readonly HttpClient _http = SupabaseHttpClient.Instance;

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var body = new { p_email = email, p_password = password };
        var response = await _http.PostAsJsonAsync("/rest/v1/rpc/login", body);
        response.EnsureSuccessStatusCode();

        var results = await response.Content.ReadFromJsonAsync<List<LoginResponse>>();
        return results?.FirstOrDefault();
    }

     public async Task<bool> Register(string nombre, string email, string telefono, string password)
    {
        var body = new
        {
            p_nombre = nombre,
            p_email = email,
            p_password = password,
            p_telefono = telefono
        };

        var response = await _http.PostAsJsonAsync("/rest/v1/rpc/registrar", body);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            await Application.Current.MainPage.DisplayAlert("ERROR", error, "OK");
            return false;
        }

        return true;
    }
}
