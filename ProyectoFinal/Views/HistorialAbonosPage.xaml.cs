using ProyectoFinal.Models;
using ProyectoFinal.Services;

namespace ProyectoFinal.Views;

public partial class HistorialAbonosPage : ContentPage
{
    private readonly Compra _compra;
    private readonly AbonoService _abonoService;

    public HistorialAbonosPage(Compra compra)
    {
        InitializeComponent();
        _compra = compra;
        _abonoService = new AbonoService();
        CargarInfo();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarAbonosAsync();
    }

    private void CargarInfo()
    {
        if (_compra.Producto != null)
        {
            ProductoLabel.Text = _compra.Producto.Nombre;
            InfoLabel.Text = $"{_compra.Producto.TipoBien} {_compra.Producto.Marca}";
        }
        else
        {
            ProductoLabel.Text = $"Compra ID: {_compra.Id}";
            InfoLabel.Text = "Información no disponible";
        }
    }

    private async Task CargarAbonosAsync()
    {
        try
        {
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;
            AbonosScrollView.IsVisible = false;

            var abonos = await _abonoService.GetAbonosByCompraAsync(_compra.Id);

            if (abonos == null || abonos.Count == 0)
            {
                EmptyFrame.IsVisible = true;
                AbonosCollection.ItemsSource = null;
            }
            else
            {
                EmptyFrame.IsVisible = false;
                AbonosCollection.ItemsSource = abonos;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los abonos: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            AbonosScrollView.IsVisible = true;
        }
    }
}
