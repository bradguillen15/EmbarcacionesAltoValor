using ProyectoFinal.Models;
using ProyectoFinal.Services;

namespace ProyectoFinal.Views;

public partial class CatalogoProductosPage : ContentPage
{
    private readonly ProductoService _productoService;

    public CatalogoProductosPage()
    {
        InitializeComponent();
        _productoService = new ProductoService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarProductosAsync();
    }

    private async Task CargarProductosAsync()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        ProductosScrollView.IsVisible = false;

        try
        {
            var productos = await _productoService.GetProductosAsync();
            ProductosCollection.ItemsSource = productos;
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            ProductosScrollView.IsVisible = true;
        }
    }

    private async void OnComprarClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Producto producto)
        {
            await Navigation.PushAsync(new RegistrarCompraPage(producto));
        }
    }
}
