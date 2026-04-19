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
        // Crear una compra temporal para usar sus propiedades calculadas
        var compraTemporal = new Compra
        {
            PrecioTotal = _precioTotal,
            PrimaInicial = _precioTotal * (_primaPorcentaje / 100),
            PlazoMeses = _plazoMeses,
            TasaInteres = _tasaInteres
        };

        MontoPrimaLabel.Text = $"₡{compraTemporal.PrimaInicial:N2}";
        MontoFinanciarLabel.Text = $"₡{compraTemporal.MontoFinanciado:N2}";

        ResumenPrecioLabel.Text = $"₡{_precioTotal:N2}";
        ResumenPrimaLabel.Text = $"₡{compraTemporal.PrimaInicial:N2}";
        ResumenFinanciarLabel.Text = $"₡{compraTemporal.MontoFinanciado:N2}";
        ResumenTasaLabel.Text = $"{_tasaInteres}%";
        ResumenCuotaLabel.Text = $"₡{compraTemporal.CuotaMensual:N2}";
        ResumenTotalConInteresLabel.Text = $"₡{compraTemporal.TotalRealAPagar:N2}";
        ResumenInteresesLabel.Text = $"Intereses totales: ₡{compraTemporal.TotalIntereses:N2}";

        if (PlazoPicker.SelectedIndex >= 0)
        {
            string plazoTexto = PlazoPicker.Items[PlazoPicker.SelectedIndex];
            ResumenPlazoLabel.Text = plazoTexto;
        }
    }

    private async void OnConfirmarClicked(object sender, EventArgs e)
    {
        // Verificar que el usuario esté logueado
        if (Session.ClienteId <= 0)
        {
            await DisplayAlert("Error", "Debe iniciar sesión para registrar una compra", "OK");
            Application.Current.MainPage = new NavigationPage(new LoginPage());
            return;
        }

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

        // Crear una compra temporal para calcular los valores del diálogo de confirmación
        var compraConfirm = new Compra
        {
            PrecioTotal = _precioTotal,
            PrimaInicial = _precioTotal * (_primaPorcentaje / 100),
            PlazoMeses = _plazoMeses,
            TasaInteres = _tasaInteres
        };

        bool confirmar = await DisplayAlert(
            "Confirmar Compra",
            $"¿Está seguro de realizar esta compra?\n\n" +
            $"Producto: {_producto.Nombre}\n" +
            $"Precio total: ₡{_precioTotal:N2}\n" +
            $"Prima inicial: ₡{compraConfirm.PrimaInicial:N2}\n" +
            $"Monto a financiar: ₡{compraConfirm.MontoFinanciado:N2}\n" +
            $"Plazo: {PlazoPicker.Items[PlazoPicker.SelectedIndex]}\n" +
            $"Tasa anual: {_tasaInteres}%\n" +
            $"Cuota mensual: ₡{compraConfirm.CuotaMensual:N2}\n" +
            $"Total a pagar (con intereses): ₡{compraConfirm.TotalRealAPagar:N2}",
            "Sí, Confirmar",
            "Cancelar"
        );

        if (!confirmar) return;

        ConfirmarButton.IsEnabled = false;
        ConfirmarButton.Text = "Procesando...";

        try
        {
            var compra = new Compra
            {
                ClienteId = Session.ClienteId,
                ProductoId = _producto.Id,
                PrecioTotal = _precioTotal,
                PrimaInicial = _precioTotal * (_primaPorcentaje / 100),
                PlazoMeses = _plazoMeses,
                TasaInteres = _tasaInteres,
                Estado = "activo",
                FechaRegistro = DateTime.UtcNow
            };

            // Usar la propiedad calculada para TotalConIntereses
            compra.TotalConIntereses = compra.TotalRealAPagar;

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
