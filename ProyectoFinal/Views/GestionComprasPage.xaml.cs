using ProyectoFinal.Models;
using ProyectoFinal.Services;

namespace ProyectoFinal.Views;

public partial class GestionComprasPage : ContentPage
{
    private readonly CompraService _compraService;

    public GestionComprasPage()
    {
        InitializeComponent();
        _compraService = new CompraService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarComprasAsync();
    }

    private async Task CargarComprasAsync()
    {
        try
        {
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;
            ComprasScrollView.IsVisible = false;

            // Verificar que el usuario esté logueado
            if (Session.ClienteId <= 0)
            {
                await DisplayAlert("Error", "Debe iniciar sesión para ver las compras", "OK");
                Application.Current.MainPage = new NavigationPage(new LoginPage());
                return;
            }

            var compras = await _compraService.GetComprasConSaldoByClienteAsync(Session.ClienteId);

            if (compras == null || compras.Count == 0)
            {
                EmptyFrame.IsVisible = true;
                ComprasCollection.ItemsSource = null;
            }
            else
            {
                EmptyFrame.IsVisible = false;
                ComprasCollection.ItemsSource = compras;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar las compras: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            ComprasScrollView.IsVisible = true;
        }
    }

    private async void OnVerDetallesClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Compra compra)
        {
            await Navigation.PushAsync(new DetalleCompraPage(compra));
        }
    }
}
