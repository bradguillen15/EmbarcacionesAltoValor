namespace ProyectoFinal.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text ?? string.Empty;
        string password = PasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Por favor ingresa tu email y contrasena", "OK");
            return;
        }

        await DisplayAlert("Datos de Login", $"Email: {email}\nContrasena: {password}", "OK");

        Application.Current.MainPage = new NavigationPage(new MenuPage());
    }

    private async void OnRegistroTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistroPage());
    }
}
