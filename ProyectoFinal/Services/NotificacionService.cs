using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ProyectoFinal.Config;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services;

public class NotificacionService
{
    private static readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("https://api.resend.com/")
    };

    public async Task EnviarNotificacionCompraAsync(Compra compra)
    {
        string emailPrimario = Preferences.Get("EmailPrimario", string.Empty);
        if (string.IsNullOrWhiteSpace(emailPrimario)) return;

        string asunto = $"✅ Compra registrada – {compra.Producto?.Nombre ?? "Embarcación"}";
        string cuerpo = $"""
            Estimado/a cliente,

            Su compra ha sido registrada exitosamente en Pura Vida Yachts.

            📋 DETALLES DE LA COMPRA
            ──────────────────────────────
            Producto:          {compra.Producto?.Nombre ?? "N/D"} – {compra.Producto?.TipoBien ?? ""}
            Marca:             {compra.Producto?.Marca ?? "N/D"}
            Precio total:      ₡{compra.PrecioTotal:N2}
            Prima inicial:     ₡{compra.PrimaInicial:N2}
            Monto financiado:  ₡{compra.MontoFinanciado:N2}
            Plazo:             {compra.PlazoMeses} meses
            Tasa anual:        {compra.TasaInteres}%
            Cuota mensual:     ₡{compra.CuotaMensual:N2}
            Total a pagar:     ₡{compra.TotalRealAPagar:N2}
            Total intereses:   ₡{compra.TotalIntereses:N2}
            Fecha de registro: {compra.FechaRegistro:dd/MM/yyyy HH:mm}
            ──────────────────────────────

            Puede realizar sus pagos desde la app en cualquier momento.

            Atentamente,
            Pura Vida Yachts
            """;

        await EnviarEmailAsync(emailPrimario, asunto, cuerpo);
    }

    public async Task EnviarNotificacionAbonoAsync(Abono abono, Compra compra)
    {
        string emailPrimario = Preferences.Get("EmailPrimario", string.Empty);
        if (string.IsNullOrWhiteSpace(emailPrimario)) return;

        decimal saldoNuevo = (compra.SaldoPendiente ?? 0) - abono.Monto;
        string tipoPagoDisplay = abono.Tipo == "extraordinario" ? "Pago Extraordinario" : "Pago Mensual";

        string asunto = $"💳 {tipoPagoDisplay} registrado – {compra.Producto?.Nombre ?? "Embarcación"}";
        string cuerpo = $"""
            Estimado/a cliente,

            Su pago ha sido registrado exitosamente en Pura Vida Yachts.

            💳 DETALLES DEL ABONO
            ──────────────────────────────
            Producto:          {compra.Producto?.Nombre ?? "N/D"} – {compra.Producto?.TipoBien ?? ""}
            Tipo de pago:      {tipoPagoDisplay}
            Monto abonado:     ₡{abono.Monto:N2}
            Fecha de pago:     {abono.FechaAbono:dd/MM/yyyy HH:mm}
            ──────────────────────────────
            Saldo anterior:    ₡{(compra.SaldoPendiente ?? 0):N2}
            Nuevo saldo:       ₡{Math.Max(0, saldoNuevo):N2}
            ──────────────────────────────
            {(saldoNuevo <= 0 ? "🎉 ¡Felicidades! Su compra ha sido LIQUIDADA completamente." : $"Cuota mensual ref.: ₡{compra.CuotaMensual:N2}")}

            Atentamente,
            Pura Vida Yachts
            """;

        await EnviarEmailAsync(emailPrimario, asunto, cuerpo);
    }

    private static async Task EnviarEmailAsync(string emailDestino, string asunto, string cuerpo)
    {
        try
        {
            string emailSecundario = Preferences.Get("EmailSecundario", string.Empty);

            var destinatarios = new List<string> { emailDestino };
            if (!string.IsNullOrWhiteSpace(emailSecundario))
                destinatarios.Add(emailSecundario);

            var payload = new ResendEmailRequest
            {
                From = AppConfig.EmailFrom,
                To = destinatarios,
                Subject = asunto,
                Text = cuerpo
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "emails");
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppConfig.ResendApiKey);
            request.Content = JsonContent.Create(payload);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[Resend] Error: {response.StatusCode} – {error}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Resend] Excepción: {ex.Message}");
        }
    }

    private class ResendEmailRequest
    {
        [JsonPropertyName("from")]
        public string From { get; set; } = string.Empty;

        [JsonPropertyName("to")]
        public List<string> To { get; set; } = new();

        [JsonPropertyName("subject")]
        public string Subject { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }
}
