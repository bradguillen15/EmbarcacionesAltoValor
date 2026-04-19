using ProyectoFinal.Models;
using ProyectoFinal.Services;

namespace ProyectoFinal.Views;

public partial class ReportePage : ContentPage
{
    private readonly CompraService _compraService;

    public ReportePage()
    {
        InitializeComponent();
        _compraService = new CompraService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarReporteAsync();
    }

    private async Task CargarReporteAsync()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        SinComprasFrame.IsVisible = false;
        ResumenFrame.IsVisible = false;
        ComprasContainer.Children.Clear();

        try
        {
            var compras = await _compraService.GetComprasConSaldoByClienteAsync(Session.ClienteId);
            var activas = compras?.Where(c => c.Estado?.ToLower() == "activo").ToList() ?? new List<Compra>();

            if (activas.Count == 0)
            {
                SinComprasFrame.IsVisible = true;
                return;
            }

            // Totales consolidados
            decimal deudaTotal = activas.Sum(c => c.SaldoPendiente ?? 0);
            decimal totalPagado = activas.Sum(c => c.TotalAbonado);
            decimal totalConIntereses = activas.Sum(c => c.TotalRealAPagar);

            DeudaTotalLabel.Text = $"₡{deudaTotal:N2}";
            TotalPagadoLabel.Text = $"₡{totalPagado:N2}";
            ComprasActivasLabel.Text = activas.Count.ToString();
            TotalConInteresesLabel.Text = $"₡{totalConIntereses:N2}";
            ResumenFrame.IsVisible = true;

            // Tarjeta por compra
            foreach (var compra in activas)
            {
                ComprasContainer.Children.Add(CrearTarjetaCompra(compra));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en ReportePage: {ex.Message}");
            await DisplayAlert("Error", "No se pudo generar el reporte.", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private Frame CrearTarjetaCompra(Compra compra)
    {
        var frame = new Frame
        {
            Padding = new Thickness(0),
            CornerRadius = 15,
            BorderColor = Colors.Transparent,
            BackgroundColor = Colors.White,
            HasShadow = true
        };

        var contenido = new VerticalStackLayout { Spacing = 0 };

        // Header
        var header = new Frame
        {
            Padding = new Thickness(15, 12),
            BorderColor = Colors.Transparent,
            BackgroundColor = (Color)Application.Current.Resources["Primary"],
            HasShadow = false
        };
        header.Content = new Label
        {
            Text = $"🚤 {compra.Producto?.Nombre ?? compra.Display}",
            FontSize = 15,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White
        };
        contenido.Children.Add(header);

        var cuerpo = new VerticalStackLayout { Padding = new Thickness(15), Spacing = 12 };

        // Barra de progreso
        var progresoLayout = new VerticalStackLayout { Spacing = 5 };
        progresoLayout.Children.Add(new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            Children =
            {
                new Label
                {
                    Text = "Progreso de pago",
                    FontSize = 13,
                    TextColor = (Color)Application.Current.Resources["Gray600"]
                },
                new Label
                {
                    Text = $"{compra.PorcentajePagado:F1}%",
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = (Color)Application.Current.Resources["Secondary"],
                    HorizontalOptions = LayoutOptions.End
                }
            }
        });
        ((Grid)progresoLayout.Children[0]).Children[1].SetValue(Grid.ColumnProperty, 1);
        progresoLayout.Children.Add(new ProgressBar
        {
            Progress = compra.PorcentajePagado / 100,
            ProgressColor = (Color)Application.Current.Resources["Secondary"],
            HeightRequest = 10
        });
        cuerpo.Children.Add(progresoLayout);

        // Grid con datos clave
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            },
            ColumnSpacing = 10,
            RowSpacing = 8,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            }
        };

        AgregarCelda(grid, 0, 0, "⏳ Saldo Pendiente", $"₡{(compra.SaldoPendiente ?? 0):N2}", Color.FromArgb("#DC2626"));
        AgregarCelda(grid, 0, 1, "✅ Abonado", $"₡{compra.TotalAbonado:N2}", Color.FromArgb("#059669"));
        AgregarCelda(grid, 1, 0, "📅 Cuota Mensual", $"₡{compra.CuotaMensual:N2}", Color.FromArgb("#4F46E5"));
        AgregarCelda(grid, 1, 1, "💳 Total c/Intereses", $"₡{compra.TotalRealAPagar:N2}", Color.FromArgb("#BE123C"));

        cuerpo.Children.Add(grid);
        contenido.Children.Add(cuerpo);
        frame.Content = contenido;
        return frame;
    }

    private static void AgregarCelda(Grid grid, int row, int col, string titulo, string valor, Color color)
    {
        var frame = new Frame
        {
            Padding = new Thickness(10),
            CornerRadius = 8,
            BorderColor = Colors.Transparent,
            BackgroundColor = Color.FromArgb("#F8FAFC"),
            HasShadow = false
        };
        var stack = new VerticalStackLayout { Spacing = 3 };
        stack.Children.Add(new Label { Text = titulo, FontSize = 11, TextColor = Color.FromArgb("#6B7280") });
        stack.Children.Add(new Label { Text = valor, FontSize = 14, FontAttributes = FontAttributes.Bold, TextColor = color });
        frame.Content = stack;
        Grid.SetRow(frame, row);
        Grid.SetColumn(frame, col);
        grid.Children.Add(frame);
    }
}
