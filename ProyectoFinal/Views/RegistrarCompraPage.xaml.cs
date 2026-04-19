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
    private int _plazoMeses = 12;

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
        PrecioBaseLabel.Text = $"₡{_precioTotal:N2}";

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
        _plazoMeses = ObtenerPlazoMeses();
        ActualizarCalculos();
    }

    private void ActualizarCalculos()
    {
        decimal montoPrima = _precioTotal * (_primaPorcentaje / 100);
        decimal montoFinanciar = _precioTotal - montoPrima;

        // Cálculo de amortización
        decimal tasaMensual = _tasaInteres / 12 / 100;
        decimal cuotaMensual = tasaMensual > 0 && _plazoMeses > 0
            ? montoFinanciar * tasaMensual / (1 - (decimal)Math.Pow((double)(1 + tasaMensual), -_plazoMeses))
            : (_plazoMeses > 0 ? montoFinanciar / _plazoMeses : 0);
        decimal totalConIntereses = cuotaMensual * _plazoMeses;
        decimal totalIntereses = totalConIntereses - montoFinanciar;

        MontoPrimaLabel.Text = $"₡{montoPrima:N2}";
        MontoFinanciarLabel.Text = $"₡{montoFinanciar:N2}";

        ResumenPrecioLabel.Text = $"₡{_precioTotal:N2}";
        ResumenPrimaLabel.Text = $"₡{montoPrima:N2}";
        ResumenFinanciarLabel.Text = $"₡{montoFinanciar:N2}";
        ResumenTasaLabel.Text = $"{_tasaInteres}%";
        ResumenCuotaLabel.Text = $"₡{cuotaMensual:N2}";
        ResumenTotalConInteresLabel.Text = $"₡{totalConIntereses:N2}";
        ResumenInteresesLabel.Text = $"Intereses totales: ₡{totalIntereses:N2}";

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

        decimal montoPrimaConfirm = _precioTotal * (_primaPorcentaje / 100);
        decimal montoFinanciarConfirm = _precioTotal - montoPrimaConfirm;
        decimal tasaMensualConfirm = _tasaInteres / 12 / 100;
        decimal cuotaConfirm = tasaMensualConfirm > 0 && _plazoMeses > 0
            ? montoFinanciarConfirm * tasaMensualConfirm / (1 - (decimal)Math.Pow((double)(1 + tasaMensualConfirm), -_plazoMeses))
            : (_plazoMeses > 0 ? montoFinanciarConfirm / _plazoMeses : 0);
        decimal totalConfirm = cuotaConfirm * _plazoMeses;

        bool confirmar = await DisplayAlert(
            "Confirmar Compra",
            $"¿Está seguro de realizar esta compra?\n\n" +
            $"Producto: {_producto.Nombre}\n" +
            $"Precio total: ₡{_precioTotal:N2}\n" +
            $"Prima inicial: ₡{montoPrimaConfirm:N2}\n" +
            $"Monto a financiar: ₡{montoFinanciarConfirm:N2}\n" +
            $"Plazo: {PlazoPicker.Items[PlazoPicker.SelectedIndex]}\n" +
            $"Tasa anual: {_tasaInteres}%\n" +
            $"Cuota mensual: ₡{cuotaConfirm:N2}\n" +
            $"Total a pagar (con intereses): ₡{totalConfirm:N2}",
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
            decimal montoFinanciar = _precioTotal - montoPrima;
            decimal tasaMensual = _tasaInteres / 12 / 100;
            decimal cuota = tasaMensual > 0 && plazoMeses > 0
                ? montoFinanciar * tasaMensual / (1 - (decimal)Math.Pow((double)(1 + tasaMensual), -plazoMeses))
                : (plazoMeses > 0 ? montoFinanciar / plazoMeses : 0);
            decimal totalConIntereses = cuota * plazoMeses;

            var compra = new Compra
            {
                ClienteId = Session.ClienteId,
                ProductoId = _producto.Id,
                PrecioTotal = _precioTotal,
                PrimaInicial = montoPrima,
                PlazoMeses = plazoMeses,
                TasaInteres = _tasaInteres,
                TotalConIntereses = totalConIntereses,
                Estado = "activo",
                FechaRegistro = DateTime.UtcNow
            };

            bool resultado = await _compraService.RegistrarCompraAsync(compra);

            if (resultado)
            {
                // Notificación automática en background
                _ = Task.Run(async () =>
                {
                    compra.Producto = _producto;
                    await new NotificacionService().EnviarNotificacionCompraAsync(compra);
                });

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
