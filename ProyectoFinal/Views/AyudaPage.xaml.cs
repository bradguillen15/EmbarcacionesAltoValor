using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace ProyectoFinal.Views;

public partial class AyudaPage : ContentPage
{
    public AyudaPage()
    {
        InitializeComponent();
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    

    private async void OnContactarClicked(object sender, EventArgs e)
    {
        try
        {
            var message = new EmailMessage
            {
                Subject = "Soporte - Pura Vida Yachts",
                Body = "Hola, necesito ayuda con la aplicación...",
                To = new List<string> { "soporte@puravidayachts.com" }
            };

            await Email.Default.ComposeAsync(message);
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "No se pudo abrir el correo.", "OK");
        }
    }
