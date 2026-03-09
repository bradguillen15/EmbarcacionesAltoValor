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
}
