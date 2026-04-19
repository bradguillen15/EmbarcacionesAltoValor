using ProyectoFinal.Models;
using ProyectoFinal.Services;

namespace ProyectoFinal.Views;

public partial class DetalleCompraPage : ContentPage
{
    private readonly Compra _compra;
    private readonly CompraService _compraService;

    public DetalleCompraPage(Compra compra)
    {
        InitializeComponent();
        _compra = compra;
        _compraService = new CompraService();
        CargarDatos();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ActualizarSaldoAsync();
    }

    private async Task ActualizarSaldoAsync()
    {
        var compraConSaldo = await _compraService.GetCompraConSaldoAsync(_compra.Id);
        if (compraConSaldo != null)
        {
            _compra.SaldoPendiente = compraConSaldo.SaldoPendiente;
            _compra.Estado = compraConSaldo.Estado;
            EstadoLabel.Text = _compra.Estado;
            ActualizarDisplaySaldo();
        }
    }

    private void CargarDatos()
    {
        // Información del producto
        ProductoNombreLabel.Text = _compra.Producto?.Nombre ?? "Producto no disponible";
        ProductoDetalleLabel.Text = _compra.Producto != null
            ? $"{_compra.Producto.TipoBien} {_compra.Producto.Marca}"
            : "Sin detalles";

        // Información financiera
        PrecioTotalLabel.Text = $"₡{_compra.PrecioTotal:N2}";
        PrimaInicialLabel.Text = $"₡{_compra.PrimaInicial:N2}";
        MontoFinanciadoLabel.Text = $"₡{_compra.MontoFinanciado:N2}";
        PlazoLabel.Text = $"{_compra.PlazoMeses} meses";
        TasaLabel.Text = $"{_compra.TasaInteres}%";
        EstadoLabel.Text = _compra.Estado;
        FechaLabel.Text = _compra.FechaRegistro.ToString("dd/MM/yyyy");
        CuotaMensualLabel.Text = $"₡{_compra.CuotaMensual:N2}";
        TotalConInteresesLabel.Text = $"₡{_compra.TotalRealAPagar:N2}";

        ActualizarDisplaySaldo();
    }

    private void ActualizarDisplaySaldo()
    {
        if (_compra.SaldoPendiente.HasValue)
        {
            SaldoPendienteLabel.Text = $"₡{_compra.SaldoPendiente.Value:N2}";
            TotalAbonadoLabel.Text = $"₡{_compra.TotalAbonado:N2}";
            ProgressBar.Progress = _compra.PorcentajePagado / 100;
            PorcentajePagadoLabel.Text = $"{_compra.PorcentajePagado:F1}% Pagado";
        }
        else
        {
            SaldoPendienteLabel.Text = "Cargando...";
            TotalAbonadoLabel.Text = "₡0.00";
            ProgressBar.Progress = 0;
            PorcentajePagadoLabel.Text = "0% Pagado";
        }

        bool esLiquidado = _compra.Estado?.ToLower() == "liquidado";
        PagoMensualButton.IsVisible = !esLiquidado;
        PagoExtraordinarioButton.IsVisible = !esLiquidado;
        ProgramarPagoButton.IsVisible = !esLiquidado;
    }

    private async void OnRealizarPagoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RealizarPagoDirectoPage(_compra, "mensualidad"));
    }

    private async void OnPagoExtraordinarioClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RealizarPagoDirectoPage(_compra, "extraordinario"));
    }

    private async void OnProgramarPagoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProgramarPagoPage(_compra, "programada"));
    }

    private async void OnVerHistorialClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HistorialAbonosPage(_compra));
    }
}
