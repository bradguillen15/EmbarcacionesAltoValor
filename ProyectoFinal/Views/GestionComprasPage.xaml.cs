namespace ProyectoFinal.Views;

public partial class GestionComprasPage : ContentPage
{
    public GestionComprasPage()
    {
        InitializeComponent();
    }
    private async void OnHacerPagoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RealizarPagoPage());
    }
    private async void OnProgramarPagoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProgramarPagoPage());
    }
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MenuPage());
    }
    
}
