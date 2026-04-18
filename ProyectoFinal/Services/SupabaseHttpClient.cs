using ProyectoFinal.Config;

namespace ProyectoFinal.Services;

/// <summary>
/// Singleton para el HttpClient de Supabase.
/// Reutiliza la misma instancia en toda la aplicación para evitar agotamiento de sockets.
/// </summary>
public static class SupabaseHttpClient
{
    private static readonly HttpClient _instance = CreateHttpClient();

    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(AppConfig.SupabaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };

        client.DefaultRequestHeaders.Add("apikey", AppConfig.SupabaseAnonKey);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AppConfig.SupabaseAnonKey}");

        return client;
    }

    /// <summary>
    /// Obtiene la instancia singleton del HttpClient configurado para Supabase.
    /// </summary>
    public static HttpClient Instance => _instance;
}
