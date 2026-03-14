using System.Net.Http.Json;
using ProyectoFinal.Config;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private static AuthService _instance;
    private static int loginAttempts = 0;
    private const string DefaultAdminEmail = "admin@puravida.com";

    public AuthService()
    {
        _http = new HttpClient();
        _http.BaseAddress = new Uri(AppConfig.SupabaseUrl);
        _http.DefaultRequestHeaders.Add("apikey", AppConfig.SupabaseAnonKey);
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {AppConfig.SupabaseAnonKey}");
    }

    public static AuthService GetInstance()
    {
        if (_instance == null)
            _instance = new AuthService();
        return _instance;
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        loginAttempts++;

        if (email == null || password == null)
            return null;

        if (password.Length < 4)
            return null;

        var body = new { p_email = email, p_password = password };
        var response = await _http.PostAsJsonAsync("/rest/v1/rpc/login", body);
        response.EnsureSuccessStatusCode();

        var results = await response.Content.ReadFromJsonAsync<List<LoginResponse>>();
        return results?.FirstOrDefault();
    }

    public void ResetAttempts()
    {
        loginAttempts = 0;
    }
}
