using ProyectoFinal.Services;
using ProyectoFinal.Models;
namespace ProyectoFinal.Views;

public partial class ProgramarPagoPage : ContentPage
{
	public ProgramarPagoPage()
	{
		InitializeComponent();
        BT_Programar_Pago.Clicked += BT_Programar_Pago_Clicked;
        BtnSalir.Clicked += BtnSalir_Clicked;
    }

    //carga la pagina al cargar la aplicacion
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            var service = new CompraService();
            var compras = await service.GetComprasConSaldoByClienteAsync(Session.ClienteId);

            PickerVehiculo.ItemsSource = compras;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void BT_Programar_Pago_Clicked(object sender, EventArgs e)
    {
        DateTime fechaSeleccionada = (DateTime)BT_FechaPago.Date;
        string monto = MontoEntry.Text ?? string.Empty;

        // Validar fecha
        if (fechaSeleccionada <= DateTime.Today)
        {
            await DisplayAlert("Error", "Por favor elegir una fecha a futuro.", "Ok");
            return;
        }

        // Validar vehículo
        if (PickerVehiculo.SelectedItem == null)
        {
            await DisplayAlert("Error", "Por favor seleccione un vehículo", "OK");
            return;
        }

        // Validar monto
        if (!double.TryParse(monto, out double montoNumerico))
        {
            await DisplayAlert("Error", "Monto inválido", "OK");
            return;
        }

        var compraSeleccionada = (Compra)PickerVehiculo.SelectedItem;

        // Crear abono PROGRAMADO
        var abono = new Abono
        {
            CompraId = compraSeleccionada.Id,
            Monto = (decimal)montoNumerico,
            Tipo = "mensualidad",
            FechaAbono = fechaSeleccionada 
        };

        var service = new AbonoService();

        var result = await service.InsertAbonoAsync(abono);

        if (result)
        {
            await DisplayAlert("Confirmación",
                $"Pago programado para el {fechaSeleccionada:dd/MM/yyyy} correctamente",
                "OK");
        }
        else
        {
            await DisplayAlert("Error", "No se pudo programar el pago", "OK");
        }
    }
    private async void BtnSalir_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new GestionComprasPage());
    }
}