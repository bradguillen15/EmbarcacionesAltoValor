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
        string password = PasswordEntry.Text ?? string.Empty;
        string confirmPassword = ConfirmPasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(nombre) || 
            string.IsNullOrWhiteSpace(email) || 
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

        await DisplayAlert("Registro Exitoso", 
            $"Nombre: {nombre}\nEmail: {email}\n\nCuenta creada correctamente", "OK");

        await Navigation.PopToRootAsync();
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
