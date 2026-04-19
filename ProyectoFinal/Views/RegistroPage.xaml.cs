using ProyectoFinal.Services;
using ProyectoFinal.Models;

namespace ProyectoFinal.Views;

public partial class RegistroPage : ContentPage
{
    public RegistroPage()
    {
        InitializeComponent();
    }

    private async void OnRegistroClicked(object sender, EventArgs e)
    {
        string nombre = NombreEntry.Text ?? string.Empty;
        string email = EmailEntry.Text ?? string.Empty;
        string telefono = TelefonoEntry.Text ?? string.Empty;
        string password = PasswordEntry.Text ?? string.Empty;
        string confirmPassword = ConfirmPasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(nombre) || 
            string.IsNullOrWhiteSpace(email) || 
            string.IsNullOrWhiteSpace(telefono) ||
            string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Por favor completa todos los campos", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Error", "Las contrasenas no coinciden", "OK");
            return;
        }

        var auth = new AuthService();

        var ok = await auth.Register(nombre, email, telefono, password);

        if (ok)
        {
            await DisplayAlert("OK", "Usuario registrado exitosamente", "OK");

            // Hacer login automático para establecer la sesión
            try
            {
                var user = await auth.LoginAsync(email, password);

                if (user != null)
                {
                    Session.ClienteId = user.Id;
                    Session.ClienteEmail = user.Email;
                    Preferences.Set("UserId", user.Id.ToString());
                    Preferences.Set("UserName", user.Nombre);
                    Preferences.Set("UserEmail", user.Email);
                    // Actualizar siempre el EmailPrimario con el correo del usuario registrado
                    Preferences.Set("EmailPrimario", user.Email);

                    Application.Current.MainPage = new NavigationPage(new MenuPage());
                }
                else
                {
                    await DisplayAlert("Aviso", "Usuario registrado, por favor inicia sesión", "OK");
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Aviso", $"Usuario registrado, por favor inicia sesión. {ex.Message}", "OK");
                await Navigation.PopAsync();
            }
        }
        else
            await DisplayAlert("Error", "No se registró", "OK");

    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
