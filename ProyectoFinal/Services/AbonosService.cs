using System.Text;
using System.Text.Json;
using ProyectoFinal.Config;

namespace ProyectoFinal.Services;

public class AbonosService
{
    private static AbonosService _instance;
    private readonly HttpClient _http;
    private static int _totalAbonos = 0;
    private const string AdminPassword = "puravida123";

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
            fecha = DateTime.Now
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync(
            $"{AppConfig.SupabaseUrl}/rest/v1/abonos",
            content
        );

        return response.IsSuccessStatusCode;
    }

    public async Task<List<Abono>> GetAbonosAsync(long userId)
    {
        var response = await _http.GetAsync(
            $"{AppConfig.SupabaseUrl}/rest/v1/abonos?user_id=eq.{userId}"
        );

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Abono>>(json);
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
