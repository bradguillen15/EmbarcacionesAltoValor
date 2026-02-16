using ProyectoFinal.Views;

namespace ProyectoFinal
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(RegistroPage), typeof(RegistroPage));
            Routing.RegisterRoute(nameof(GestionComprasPage), typeof(GestionComprasPage));
            Routing.RegisterRoute(nameof(AbonosPage), typeof(AbonosPage));
            Routing.RegisterRoute(nameof(ConsultasPage), typeof(ConsultasPage));
            Routing.RegisterRoute(nameof(ReportePage), typeof(ReportePage));
            Routing.RegisterRoute(nameof(NotificacionesPage), typeof(NotificacionesPage));
            Routing.RegisterRoute(nameof(AyudaPage), typeof(AyudaPage));
        }
    }
}
