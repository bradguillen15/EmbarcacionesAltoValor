using ProyectoFinal.Models;
using ProyectoFinal.Services;

namespace ProyectoFinal.Views;

public partial class RealizarPagoDirectoPage : ContentPage
{
    private readonly Compra _compra;
    private readonly string _tipoPago;
    private readonly CompraService _compraService;
    private decimal? _saldoPendiente;

    public RealizarPagoDirectoPage(Compra compra, string tipoPago)
    {
        InitializeComponent();
        _compra = compra;
        _tipoPago = tipoPago;
        _compraService = new CompraService();
        CargarDatos();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarSaldoAsync();
    }

    private async Task CargarSaldoAsync()
    {
        try
        {
            var compraConSaldo = await _compraService.GetCompraConSaldoAsync(_compra.Id);
            if (compraConSaldo != null && compraConSaldo.SaldoPendiente.HasValue)
            {
                _saldoPendiente = compraConSaldo.SaldoPendiente.Value;
                SaldoPendienteLabel.Text = $"Saldo pendiente: ${_saldoPendiente.Value:N2}";
                SaldoPendienteLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al cargar saldo: {ex.Message}");
        }
    }

    private void CargarDatos()
    {
        HeaderLabel.Text = _tipoPago == "extraordinario" 
            ? "💰 Pago Extraordinario" 
            : "💳 Pago Mensual";

        if (_compra.Producto != null)
        {
            ProductoLabel.Text = $"{_compra.Producto.Nombre} - {_compra.Producto.TipoBien}";
        }
        else
        {
            ProductoLabel.Text = $"Compra ID: {_compra.Id}";
        }
    }

    private async void OnConfirmarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(MontoEntry.Text))
        {
            await DisplayAlert("Error", "Ingrese un monto", "OK");
            return;
        }

        if (!decimal.TryParse(MontoEntry.Text, out decimal monto) || monto <= 0)
        {
            await DisplayAlert("Error", "Monto inválido", "OK");
            return;
        }

        if (_saldoPendiente.HasValue && monto > _saldoPendiente.Value)
        {
            bool pagarTotal = await DisplayAlert(
                "Monto Excedido", 
                $"El monto ingresado (${monto:N2}) excede el saldo pendiente (${_saldoPendiente.Value:N2}).\n\n" +
                $"¿Desea pagar el saldo total de ${_saldoPendiente.Value:N2}?",
                "Sí, pagar total",
                "Cancelar"
            );

            if (!pagarTotal)
                return;

            monto = _saldoPendiente.Value;
            MontoEntry.Text = monto.ToString("F2");
        }

        if (MetodoPagoPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Error", "Seleccione un método de pago", "OK");
            return;
        }

        var tipoPagoTexto = _tipoPago == "extraordinario" ? "extraordinario" : "mensualidad";
        var confirmar = await DisplayAlert("Confirmar", 
            $"¿Confirmar {tipoPagoTexto} de ${monto:N2}?", 
            "Sí", "No");

        if (!confirmar)
            return;

        var abono = new Abono
        {
            CompraId = _compra.Id,
            Monto = monto,
            Tipo = tipoPagoTexto,
            FechaAbono = DateTime.UtcNow
        };

        var abonoService = new AbonoService();
        var resultado = await abonoService.InsertAbonoAsync(abono);

        if (resultado)
        {
            decimal nuevoSaldo = _saldoPendiente.Value - monto;
            string mensaje;

            if (nuevoSaldo <= 0)
            {
                mensaje = "¡Felicidades! 🎉\n\nHa liquidado completamente su compra.\nSaldo pendiente: $0.00";
                _compra.Estado = "liquidado";
                _compra.SaldoPendiente = 0;
            }
            else
            {
                mensaje = $"Pago registrado correctamente.\n\nSaldo pendiente: ${nuevoSaldo:N2}";
                _compra.SaldoPendiente = nuevoSaldo;
            }

            await DisplayAlert("Éxito", mensaje, "OK");
            await Navigation.PopToRootAsync();
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
