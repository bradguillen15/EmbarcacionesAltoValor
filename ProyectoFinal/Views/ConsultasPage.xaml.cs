using ProyectoFinal.Models;
using ProyectoFinal.Services;

namespace ProyectoFinal.Views;

public partial class ConsultasPage : ContentPage
{
    private readonly CompraService _compraService;
    private readonly AbonoService _abonoService;

    public ConsultasPage()
    {
        InitializeComponent();
        _compraService = new CompraService();
        _abonoService = new AbonoService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarConsultasAsync();
    }

    private async Task CargarConsultasAsync()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        SinComprasFrame.IsVisible = false;
        ComprasContainer.Children.Clear();

        try
        {
            var compras = await _compraService.GetComprasConSaldoByClienteAsync(Session.ClienteId);

            if (compras == null || compras.Count == 0)
            {
                SinComprasFrame.IsVisible = true;
                return;
            }

            foreach (var compra in compras)
            {
                var abonos = await _abonoService.GetAbonosByCompraAsync(compra.Id);
                int cantidadAbonos = abonos?.Count ?? 0;

                // Fecha estimada de finalización
                string fechaEstimada = CalcularFechaEstimada(compra, cantidadAbonos);

                var card = CrearTarjetaConsulta(compra, cantidadAbonos, fechaEstimada);
                ComprasContainer.Children.Add(card);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en ConsultasPage: {ex.Message}");
            await DisplayAlert("Error", "No se pudieron cargar las consultas.", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private string CalcularFechaEstimada(Compra compra, int cantidadAbonos)
    {
        if (!compra.SaldoPendiente.HasValue || compra.SaldoPendiente.Value <= 0)
            return "Liquidado";

        if (compra.CuotaMensual <= 0)
            return "N/D";

        // Meses restantes = saldo / cuota mensual (redondeado hacia arriba)
        int mesesRestantes = (int)Math.Ceiling((double)(compra.SaldoPendiente.Value / compra.CuotaMensual));
        var fechaEstimada = DateTime.Today.AddMonths(mesesRestantes);
        return fechaEstimada.ToString("MMMM yyyy");
    }

    private Frame CrearTarjetaConsulta(Compra compra, int cantidadAbonos, string fechaEstimada)
    {
        bool esLiquidado = compra.Estado?.ToLower() == "liquidado";
        string saldoTexto = compra.SaldoPendiente.HasValue
            ? $"₡{compra.SaldoPendiente.Value:N2}"
            : "Cargando...";

        var frame = new Frame
        {
            Padding = new Thickness(0),
            CornerRadius = 15,
            BorderColor = Colors.Transparent,
            BackgroundColor = Colors.White,
            HasShadow = true
        };

        var contenido = new VerticalStackLayout { Spacing = 0 };

        // Header de la tarjeta
        var header = new Frame
        {
            Padding = new Thickness(15, 12),
            BorderColor = Colors.Transparent,
            BackgroundColor = esLiquidado
                ? Color.FromArgb("#059669")
                : (Color)Application.Current.Resources["Primary"],
            HasShadow = false
        };
        header.Content = new Label
        {
            Text = $"🚤 {compra.Producto?.Nombre ?? compra.Display}",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White
        };
        contenido.Children.Add(header);

        // Cuerpo con datos
        var cuerpo = new VerticalStackLayout { Padding = new Thickness(15), Spacing = 12 };

        // Fila: estado
        var estadoBadge = new Frame
        {
            Padding = new Thickness(10, 5),
            CornerRadius = 8,
            BorderColor = Colors.Transparent,
            BackgroundColor = esLiquidado ? Color.FromArgb("#ECFDF5") : Color.FromArgb("#FEF3C7"),
            HasShadow = false,
            HorizontalOptions = LayoutOptions.Start
        };
        estadoBadge.Content = new Label
        {
            Text = $"Estado: {compra.Estado?.ToUpper()}",
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextColor = esLiquidado ? Color.FromArgb("#059669") : Color.FromArgb("#D97706")
        };
        cuerpo.Children.Add(estadoBadge);

        // Grid de datos financieros
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            },
            ColumnSpacing = 10,
            RowSpacing = 10,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            }
        };

        AgregarFila(grid, 0, 0, "⏳ Saldo pendiente", saldoTexto, Color.FromArgb("#DC2626"));
        AgregarFila(grid, 0, 1, "✅ Total abonado", $"₡{compra.TotalAbonado:N2}", Color.FromArgb("#059669"));
        AgregarFila(grid, 1, 0, "📋 Abonos realizados", $"{cantidadAbonos}", Color.FromArgb("#0277BD"));
        AgregarFila(grid, 1, 1, "📅 Cuota mensual", $"₡{compra.CuotaMensual:N2}", Color.FromArgb("#4F46E5"));
        AgregarFila(grid, 2, 0, "🏁 Fecha estimada fin", fechaEstimada, Color.FromArgb("#6B7280"));
        AgregarFila(grid, 2, 1, "💳 Total c/intereses", $"₡{compra.TotalRealAPagar:N2}", Color.FromArgb("#BE123C"));

        cuerpo.Children.Add(grid);

        // Barra de progreso
        var progresoLayout = new VerticalStackLayout { Spacing = 5 };
        var progresoPct = new Label
        {
            Text = $"Progreso: {compra.PorcentajePagado:F1}%",
            FontSize = 13,
            TextColor = (Color)Application.Current.Resources["Secondary"],
            FontAttributes = FontAttributes.Bold
        };
        var barra = new ProgressBar
        {
            Progress = compra.PorcentajePagado / 100,
            ProgressColor = (Color)Application.Current.Resources["Secondary"],
            HeightRequest = 10
        };
        progresoLayout.Children.Add(progresoPct);
        progresoLayout.Children.Add(barra);
        cuerpo.Children.Add(progresoLayout);

        contenido.Children.Add(cuerpo);
        frame.Content = contenido;
        return frame;
    }

    private static void AgregarFila(Grid grid, int row, int col, string titulo, string valor, Color colorValor)
    {
        var celda = new Frame
        {
            Padding = new Thickness(10),
            CornerRadius = 10,
            BorderColor = Colors.Transparent,
            BackgroundColor = Color.FromArgb("#F8FAFC"),
            HasShadow = false
        };
        var stack = new VerticalStackLayout { Spacing = 3 };
        stack.Children.Add(new Label
        {
            Text = titulo,
            FontSize = 11,
            TextColor = Color.FromArgb("#6B7280")
        });
        stack.Children.Add(new Label
        {
            Text = valor,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = colorValor
        });
        celda.Content = stack;
        Grid.SetRow(celda, row);
        Grid.SetColumn(celda, col);
        grid.Children.Add(celda);
    }
}
