using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ProyectoFinal.Config;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services;

public class NotificacionService
{
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

            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress(AppConfig.SmtpFromName, AppConfig.SmtpUser));
            mensaje.To.Add(MailboxAddress.Parse(emailDestino));

            if (!string.IsNullOrWhiteSpace(emailSecundario))
                mensaje.Cc.Add(MailboxAddress.Parse(emailSecundario));

            mensaje.Subject = asunto;
            mensaje.Body = new TextPart("plain") { Text = cuerpo };

            using var client = new SmtpClient();
            await client.ConnectAsync(AppConfig.SmtpHost, AppConfig.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(AppConfig.SmtpUser, AppConfig.SmtpPassword);
            await client.SendAsync(mensaje);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NotificacionService] Error enviando email: {ex.Message}");
        }
    }
}
