namespace ProyectoFinal.Views;

public partial class MenuPage : ContentPage
{
    public MenuPage()
    {
        InitializeComponent();
    }

    private async void OnGestionComprasClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(GestionComprasPage));
    }

    private async void OnAbonosClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AbonosPage));
    }

    private async void OnConsultasClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ConsultasPage));
    }

    private async void OnReporteClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ReportePage));
    }

    private async void OnNotificacionesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NotificacionesPage));
    }

    private async void OnAyudaClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AyudaPage");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Cerrar Sesion", "Estas seguro que deseas cerrar sesion?", "Si", "No");
        
        if (confirm)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
