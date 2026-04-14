using ProyectoFinal.Models;
using ProyectoFinal.Services;

namespace ProyectoFinal.Views;

public partial class RegistrarCompraPage : ContentPage
{
    private readonly Producto _producto;
    private readonly CompraService _compraService;
    private decimal _precioTotal;
    private decimal _primaPorcentaje = 20;
    private decimal _tasaInteres = 15;

    public RegistrarCompraPage(Producto producto)
    {
        InitializeComponent();

        _producto = producto;
        _compraService = new CompraService();

        if (!producto.PrecioBase.HasValue || producto.PrecioBase.Value <= 0)
        {
            Application.Current.MainPage.DisplayAlert("Error", "Este producto no tiene un precio válido.", "OK");
            Navigation.PopAsync();
            return;
        }

        _precioTotal = producto.PrecioBase.Value;

        InicializarDatos();
    }

    private void InicializarDatos()
    {
        ProductoNombreLabel.Text = _producto.Nombre;
        ProductoDetalleLabel.Text = $"{_producto.TipoBien} {_producto.Marca} • {_producto.Zona}";
        PrecioBaseLabel.Text = $"${_precioTotal:N2}";

        // Seleccionar 12 meses por defecto
        PlazoPicker.SelectedIndex = 0;

        ActualizarCalculos();
    }

    private void OnPrimaSliderChanged(object sender, ValueChangedEventArgs e)
    {
        _primaPorcentaje = (decimal)Math.Round(e.NewValue, 0);
        PrimaPorcentajeLabel.Text = $"{_primaPorcentaje}%";
        ActualizarCalculos();
    }

    private void OnTasaInteresChanged(object sender, TextChangedEventArgs e)
    {
        if (decimal.TryParse(e.NewTextValue, out decimal tasa))
        {
            _tasaInteres = tasa;

            if (tasa < 10)
            {
                TasaErrorLabel.IsVisible = true;
                ConfirmarButton.IsEnabled = false;
                ConfirmarButton.BackgroundColor = Colors.Gray;
            }
            else
            {
                TasaErrorLabel.IsVisible = false;
                ConfirmarButton.IsEnabled = true;
                ConfirmarButton.BackgroundColor = (Color)Application.Current.Resources["Primary"];
            }

            ActualizarCalculos();
        }
        else
        {
            TasaErrorLabel.IsVisible = true;
            TasaErrorLabel.Text = "⚠️ Ingrese un valor numérico válido";
            ConfirmarButton.IsEnabled = false;
            ConfirmarButton.BackgroundColor = Colors.Gray;
        }
    }

    private void OnPlazoSelectionChanged(object sender, EventArgs e)
    {
        ActualizarCalculos();
    }

    private void ActualizarCalculos()
    {
        decimal montoPrima = _precioTotal * (_primaPorcentaje / 100);
        decimal montoFinanciar = _precioTotal - montoPrima;

        MontoPrimaLabel.Text = $"${montoPrima:N2}";
        MontoFinanciarLabel.Text = $"${montoFinanciar:N2}";

        ResumenPrecioLabel.Text = $"${_precioTotal:N2}";
        ResumenPrimaLabel.Text = $"${montoPrima:N2}";
        ResumenFinanciarLabel.Text = $"${montoFinanciar:N2}";
        ResumenTasaLabel.Text = $"{_tasaInteres}%";

        if (PlazoPicker.SelectedIndex >= 0)
        {
            string plazoTexto = PlazoPicker.Items[PlazoPicker.SelectedIndex];
            ResumenPlazoLabel.Text = plazoTexto;
        }
    }

    private async void OnConfirmarClicked(object sender, EventArgs e)
    {
        if (PlazoPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Error", "Por favor seleccione un plazo de financiamiento", "OK");
            return;
        }

        if (_tasaInteres < 10)
        {
            await DisplayAlert("Error", "La tasa de interés debe ser al menos 10%", "OK");
            return;
        }

        bool confirmar = await DisplayAlert(
            "Confirmar Compra",
            $"¿Está seguro de realizar esta compra?\n\n" +
            $"Producto: {_producto.Nombre}\n" +
            $"Total: ${_precioTotal:N2}\n" +
            $"Prima: ${(_precioTotal * (_primaPorcentaje / 100)):N2}\n" +
            $"Plazo: {PlazoPicker.Items[PlazoPicker.SelectedIndex]}",
            "Sí, Confirmar",
            "Cancelar"
        );

        if (!confirmar) return;

        ConfirmarButton.IsEnabled = false;
        ConfirmarButton.Text = "Procesando...";

        try
        {
            int plazoMeses = ObtenerPlazoMeses();
            decimal montoPrima = _precioTotal * (_primaPorcentaje / 100);

            var compra = new Compra
            {
                ClienteId = Session.ClienteId,
                ProductoId = _producto.Id,
                PrecioTotal = _precioTotal,
                PrimaInicial = montoPrima,
                PlazoMeses = plazoMeses,
                TasaInteres = _tasaInteres,
                Estado = "activo",
                FechaRegistro = DateTime.UtcNow
            };

            bool resultado = await _compraService.RegistrarCompraAsync(compra);

            if (resultado)
            {
                await DisplayAlert(
                    "¡Éxito!",
                    "Su compra ha sido registrada exitosamente. Puede comenzar a realizar abonos desde el menú principal.",
                    "OK"
                );

                await Navigation.PopToRootAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error inesperado: {ex.Message}", "OK");
        }
        finally
        {
            ConfirmarButton.IsEnabled = true;
            ConfirmarButton.Text = "Confirmar Compra";
        }
    }

    private int ObtenerPlazoMeses()
    {
        return PlazoPicker.SelectedIndex switch
        {
            0 => 12,
            1 => 24,
            2 => 36,
            3 => 48,
            4 => 60,
            _ => 12
        };
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert(
            "Cancelar",
            "¿Está seguro de cancelar esta compra?",
            "Sí",
            "No"
        );

        if (confirmar)
        {
            await Navigation.PopAsync();
        }
    }
}
