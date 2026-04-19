using ProyectoFinal.Services;

namespace ProyectoFinal.Views;

public partial class NotificacionesPage : ContentPage
{
    public NotificacionesPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        EmailPrimarioEntry.Text = Preferences.Get("EmailPrimario", Preferences.Get("UserEmail", string.Empty));
        EmailSecundarioEntry.Text = Preferences.Get("EmailSecundario", string.Empty);
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        string emailPrimario = EmailPrimarioEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(emailPrimario))
        {
            await DisplayAlert("Error", "El correo primario es obligatorio.", "OK");
            return;
        }

        // Validación mejorada de email
        if (!emailPrimario.Contains('@') || 
            !emailPrimario.Contains('.') ||
            emailPrimario.IndexOf('@') == 0 || 
            emailPrimario.IndexOf('@') == emailPrimario.Length - 1 ||
            emailPrimario.LastIndexOf('.') < emailPrimario.IndexOf('@'))
        {
            await DisplayAlert("Error", "Ingrese un correo válido (ej: usuario@dominio.com).", "OK");
            return;
        }

        Preferences.Set("EmailPrimario", emailPrimario);
        Preferences.Set("EmailSecundario", EmailSecundarioEntry.Text?.Trim() ?? string.Empty);

        await DisplayAlert("✅ Guardado", "Configuración de correos guardada correctamente.", "OK");
    }

    private async void OnProbarClicked(object sender, EventArgs e)
    {
        string emailPrimario = Preferences.Get("EmailPrimario", string.Empty);
        if (string.IsNullOrWhiteSpace(emailPrimario))
        {
            await DisplayAlert("Error", "Primero guarde un correo primario.", "OK");
            return;
        }

        EnviandoIndicator.IsRunning = true;
        EnviandoIndicator.IsVisible = true;

        try
        {
            var compra = new Models.Compra
            {
                PrecioTotal = 8500000,
                PrimaInicial = 1700000,
                PlazoMeses = 36,
                TasaInteres = 15,
                TotalConIntereses = 10200000,
                Estado = "activo",
                FechaRegistro = DateTime.Now,
                Producto = new Models.Producto { Nombre = "Sea-Doo GTX 300", TipoBien = "Moto acuática", Marca = "Sea-Doo" }
            };

            await new NotificacionService().EnviarNotificacionCompraAsync(compra);
            await DisplayAlert("📧 Enviado", $"Correo de prueba enviado a:\n{emailPrimario}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo enviar: {ex.Message}", "OK");
        }
        finally
        {
            EnviandoIndicator.IsRunning = false;
            EnviandoIndicator.IsVisible = false;
        }
    }
}
