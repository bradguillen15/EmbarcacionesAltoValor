using ProyectoFinal.Services;

namespace ProyectoFinal.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? string.Empty;
        string password = PasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Por favor ingresa tu email y contraseña", "OK");
            return;
        }

        LoginButton.IsEnabled = false;
        LoginButton.Text = "Iniciando sesión...";

        try
        {
            var authService = new AuthService();
            var user = await authService.LoginAsync(email, password);

            if (user != null)
            {
                Preferences.Set("UserId", user.Id.ToString());
                Preferences.Set("UserName", user.Nombre);

                Application.Current.MainPage = new NavigationPage(new MenuPage());
            }
            else
            {
                await DisplayAlert("Error", "Email o contraseña incorrectos", "OK");
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "No se pudo conectar al servidor", "OK");
        }
        finally
        {
            LoginButton.IsEnabled = true;
            LoginButton.Text = "Iniciar Sesión";
        }
    }

    private async void OnRegistroTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistroPage());
    }
}
