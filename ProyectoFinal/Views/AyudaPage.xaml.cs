namespace ProyectoFinal.Views;

public partial class AyudaPage : ContentPage
{
    public AyudaPage()
    {
        InitializeComponent();
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MenuPage");
    }
}
