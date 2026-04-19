using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ProyectoFinal.Config;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services;

public class NotificacionService
{
    private static readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("https://send.api.mailtrap.io/")
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

            // Construir lista de destinatarios
            var destinatarios = new List<MailtrapRecipient> 
            { 
                new() { Email = emailDestino }
            };

            if (!string.IsNullOrWhiteSpace(emailSecundario))
            {
                destinatarios.Add(new MailtrapRecipient { Email = emailSecundario });
            }

            var payload = new MailtrapEmailRequest
            {
                From = new MailtrapSender 
                { 
                    Email = "no-reply@demomailtrap.com", // Email por defecto de Mailtrap sandbox
                    Name = AppConfig.EmailFromName 
                },
                To = destinatarios,
                Subject = asunto,
                Text = cuerpo,
                Category = "Notificaciones"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "api/send");
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppConfig.MailtrapApiToken);
            request.Content = JsonContent.Create(payload);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[Mailtrap] Error: {response.StatusCode} – {error}");
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[Mailtrap] Email enviado exitosamente a {emailDestino}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Mailtrap] Excepción: {ex.Message}");
        }
    }

    // Clases para el formato de Mailtrap API
    private class MailtrapEmailRequest
    {
        [JsonPropertyName("from")]
        public MailtrapSender From { get; set; } = new();

        [JsonPropertyName("to")]
        public List<MailtrapRecipient> To { get; set; } = new();

        [JsonPropertyName("subject")]
        public string Subject { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;
    }

    private class MailtrapSender
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    private class MailtrapRecipient
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }
}
