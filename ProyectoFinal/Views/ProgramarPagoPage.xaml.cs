namespace ProyectoFinal.Views;

public partial class ProgramarPagoPage : ContentPage
{
	public ProgramarPagoPage()
	{
		InitializeComponent();
        BT_Programar_Pago.Clicked += BT_Programar_Pago_Clicked;
        BtnSalir.Clicked += BtnSalir_Clicked;
    }

    private async void BT_Programar_Pago_Clicked(object sender, EventArgs e)
    {
        DateTime fechaSeleccionada = (DateTime)BT_FechaPago.Date;

        if (fechaSeleccionada <= DateTime.Today)
        {
            await DisplayAlert("Error", "Por favor elegir una fecha a futuro.", "Ok");

        }

        else
        {
            await DisplayAlert("Confirmacion", $"El depósito fue programado para el {fechaSeleccionada:dd/MM/yyyy} exitosamente", "Ok");
        }
    }
    private async void BtnSalir_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new GestionComprasPage());
    }
}