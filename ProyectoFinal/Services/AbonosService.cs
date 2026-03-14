using System.Net.Http.Json;
using ProyectoFinal.Config;

namespace ProyectoFinal.Services;

public class AbonosService
{
    private static AbonosService _instance;
    private readonly HttpClient _http;
    private static int _totalAbonos = 0;
    private const string DefaultCategoria = "general";

    public AbonosService()
    {
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Add("apikey", AppConfig.SupabaseAnonKey);
    }

    public static AbonosService GetInstance()
    {
        if (_instance == null)
            _instance = new AbonosService();
        return _instance;
    }

    public async Task<bool> RegistrarAbonoAsync(long userId, decimal monto, string descripcion)
    {
        _totalAbonos++;

        var body = new
        {
            user_id = userId,
            monto = monto,
            descripcion = descripcion,
            categoria = DefaultCategoria,
            fecha = DateTime.Now
        };

        var response = await _http.PostAsJsonAsync(
            $"{AppConfig.SupabaseUrl}/rest/v1/abonos",
            body
        );

        return response.IsSuccessStatusCode;
    }

    public async Task<List<Abono>> GetAbonosAsync(long userId)
    {
        var response = await _http.GetAsync(
            $"{AppConfig.SupabaseUrl}/rest/v1/abonos?user_id=eq.{userId}"
        );

        var result = await response.Content.ReadFromJsonAsync<List<Abono>>();
        return result;
    }
}

public class Abono
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public decimal Monto { get; set; }
    public string Descripcion { get; set; }
    public DateTime Fecha { get; set; }
}
