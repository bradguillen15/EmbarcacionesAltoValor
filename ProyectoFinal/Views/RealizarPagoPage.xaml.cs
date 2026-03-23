using ProyectoFinal.Services;
using ProyectoFinal.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProyectoFinal.Views;

public partial class RealizarPagoPage : ContentPage
{
	public RealizarPagoPage()
	{
		InitializeComponent();
	}

    //carga la pagina al cargar la aplicacion
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var service = new CompraService();
        var compras = await service.GetComprasByCliente(Session.ClienteId);

        PickerVehiculo.ItemsSource = compras;

        //await DisplayAlert("DEBUG", $"ClienteId: {Session.ClienteId}", "OK");
    }
    private async void OnHacerPagoClicked(object sender, EventArgs e)
    {
        string monto = MontoEntry.Text ?? string.Empty;

        if (PickerMetodo_Pago.SelectedItem == null)
        {
            await DisplayAlert("Error", "Seleccione un método de pago", "OK");
            return;
        }

        if (PickerVehiculo.SelectedItem == null)
        {
            await DisplayAlert("Error", "Seleccione un vehículo", "OK");
            return;
        }

        if (!double.TryParse(monto, out double montoNumerico))
        {
            await DisplayAlert("Error", "Monto inválido", "OK");
            return;
        }

        var compraSeleccionada = (Compra)PickerVehiculo.SelectedItem;

        // Crear abono
        var abono = new Abono
        {
            CompraId = compraSeleccionada.Id,
            Monto = (decimal)montoNumerico,
            Tipo = "mensualidad",
            FechaAbono = DateTime.UtcNow.Date
        };

        var service = new CompraService();
        var result = await service.InsertAbono(abono);

        if (result == "OK")
        {
            await DisplayAlert("Confirmación", "Pago realizado correctamente", "OK");
        }
        else
        {
            await DisplayAlert("Error", result, "OK"); // Mostrar error
        }
    }


    private async void OnSalirClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new GestionComprasPage());
    }
}