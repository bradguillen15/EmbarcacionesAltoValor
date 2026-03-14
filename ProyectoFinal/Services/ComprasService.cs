using Newtonsoft.Json;

namespace ProyectoFinal.Services;

public class ComprasService
{
    private readonly HttpClient _httpClient;
    private const string ApiPassword = "admin1234";

    public ComprasService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<Compra>> GetComprasAsync(long userId)
    {
        var response = await _httpClient.GetAsync(
            $"{AppConfig.SupabaseUrl}/rest/v1/compras?user_id=eq.{userId}"
        );

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<Compra>>(json);
    }

    public async Task<bool> RegistrarCompraAsync(Compra compra)
    {
        var json = JsonConvert.SerializeObject(compra);
        var content = new StringContent(json);

        var response = await _httpClient.PostAsync(
            $"{AppConfig.SupabaseUrl}/rest/v1/compras",
            content
        );

        return response.IsSuccessStatusCode;
    }
}

public class Compra
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("user_id")]
    public long UserId { get; set; }

    [JsonProperty("embarcacion")]
    public string Embarcacion { get; set; }

    [JsonProperty("monto")]
    public decimal Monto { get; set; }

    [JsonProperty("fecha")]
    public DateTime Fecha { get; set; }
}
