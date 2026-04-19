using ProyectoFinal.Models;
using ProyectoFinal.Services;

namespace ProyectoFinal.Views;

public partial class AbonosPage : ContentPage
{
    private readonly AbonoService _abonoService;

    public AbonosPage()
    {
        InitializeComponent();
        _abonoService = new AbonoService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarAbonosAsync();
    }

    private async Task CargarAbonosAsync()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        SinAbonosFrame.IsVisible = false;
        AbonosContainer.Children.Clear();

        try
        {
            var items = await _abonoService.GetAllAbonosByClienteAsync(Session.ClienteId);

            if (items == null || items.Count == 0)
            {
                SinAbonosFrame.IsVisible = true;
                return;
            }

            foreach (var (abono, compra) in items)
            {
                AbonosContainer.Children.Add(CrearTarjetaAbono(abono, compra));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en AbonosPage: {ex.Message}");
            await DisplayAlert("Error", "No se pudo cargar el historial de abonos.", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private Frame CrearTarjetaAbono(Abono abono, Compra compra)
    {
        bool esExtraordinario = abono.Tipo == "extraordinario";
        var colorTipo = esExtraordinario ? Color.FromArgb("#7C3AED") : (Color)Application.Current.Resources["Primary"];
        var bgTipo = esExtraordinario ? Color.FromArgb("#F5F3FF") : Color.FromArgb("#EFF6FF");
        string emojiTipo = esExtraordinario ? "⭐" : "📅";
        string tipoDisplay = esExtraordinario ? "Pago Extraordinario" : "Pago Mensual";

        var frame = new Frame
        {
            Padding = new Thickness(15),
            CornerRadius = 12,
            BorderColor = Colors.Transparent,
            BackgroundColor = Colors.White,
            HasShadow = true
        };

        var contenido = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            ColumnSpacing = 10
        };

        // Columna izquierda: info del abono
        var infoStack = new VerticalStackLayout { Spacing = 4 };

        var badge = new Frame
        {
            Padding = new Thickness(8, 4),
            CornerRadius = 6,
            BorderColor = Colors.Transparent,
            BackgroundColor = bgTipo,
            HasShadow = false,
            HorizontalOptions = LayoutOptions.Start
        };
        badge.Content = new Label
        {
            Text = $"{emojiTipo} {tipoDisplay}",
            FontSize = 11,
            FontAttributes = FontAttributes.Bold,
            TextColor = colorTipo
        };
        infoStack.Children.Add(badge);

        infoStack.Children.Add(new Label
        {
            Text = compra.Producto?.Nombre ?? compra.Display,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = (Color)Application.Current.Resources["Gray900"]
        });

        infoStack.Children.Add(new Label
        {
            Text = $"{compra.Producto?.TipoBien ?? ""} · {compra.Producto?.Marca ?? ""}",
            FontSize = 12,
            TextColor = (Color)Application.Current.Resources["Gray600"]
        });

        infoStack.Children.Add(new Label
        {
            Text = abono.FechaAbono.ToString("dd/MM/yyyy HH:mm"),
            FontSize = 12,
            TextColor = (Color)Application.Current.Resources["Gray600"]
        });

        Grid.SetColumn(infoStack, 0);
        contenido.Children.Add(infoStack);

        // Columna derecha: monto + botón ver compra
        var montoStack = new VerticalStackLayout { Spacing = 8, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };

        montoStack.Children.Add(new Label
        {
            Text = $"₡{abono.Monto:N2}",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#059669"),
            HorizontalOptions = LayoutOptions.End
        });

        var verBtn = new Button
        {
            Text = "Ver compra",
            FontSize = 12,
            BackgroundColor = (Color)Application.Current.Resources["Primary"],
            TextColor = Colors.White,
            CornerRadius = 8,
            HeightRequest = 34,
            Padding = new Thickness(10, 0)
        };
        verBtn.Clicked += async (s, e) =>
            await Navigation.PushAsync(new DetalleCompraPage(compra));

        montoStack.Children.Add(verBtn);

        Grid.SetColumn(montoStack, 1);
        contenido.Children.Add(montoStack);

        frame.Content = contenido;
        return frame;
    }
}
