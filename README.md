# Pura Vida Yachts

App móvil de gestión financiera para compras de embarcaciones recreativas (jet ski, lanchas). Permite registrar compras financiadas, controlar abonos y recibir notificaciones automáticas por correo.

## Stack

| Capa | Tecnología |
|---|---|
| Mobile | .NET 10 MAUI — C# 14 |
| Backend | Supabase (PostgreSQL + REST API) |
| Email | Resend API |
| Plataformas | Android, iOS, macOS Catalyst, Windows |

## Arquitectura

La app sigue un patrón de capas simple sin framework MVVM — lógica directamente en code-behind. Los servicios encapsulan las llamadas HTTP a Supabase y Resend.

```
ProyectoFinal/
├── Config/         # AppConfig.cs (credenciales, gitignored)
├── Models/         # Compra, Abono, Producto, LoginResponse
├── Services/       # AuthService, CompraService, AbonoService,
│                   # ProductoService, NotificacionService
└── Views/          # Pages XAML + code-behind
```

**Sesión de usuario:** `Preferences` de MAUI — persiste `UserId`, `UserName`, `UserEmail` y los emails de notificación. Se limpia al cerrar sesión.

**Amortización francesa:** cada compra calcula cuota mensual, total con intereses e intereses totales en `Compra.cs` usando:

```
TasaMensual  = TasaAnual / 12 / 100
CuotaMensual = MontoFinanciado × TasaMensual / (1 − (1 + TasaMensual)^−PlazoMeses)
TotalAPagar  = CuotaMensual × PlazoMeses
```

`TotalConIntereses` se persiste en la tabla `compras` como valor contractual. `SaldoPendiente` se deriva en la vista `vw_compra_con_saldo` como `TotalConIntereses − SUM(abonos)`.

**Notificaciones:** `NotificacionService` hace un POST directo a `api.resend.com/emails` con el API key de Resend. Se dispara automáticamente al registrar una compra o un abono.

## Requisitos

- [Visual Studio 2022](https://visualstudio.microsoft.com/) v17.13+ con workload **.NET MAUI**
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Proyecto en [Supabase](https://supabase.com)
- Cuenta gratuita en [Resend](https://resend.com)

## Setup

### 1. Clonar y restaurar

```bash
git clone https://github.com/bradguillen15/EmbarcacionesAltoValor.git
cd EmbarcacionesAltoValor
dotnet restore
```

### 2. Credenciales

`AppConfig.cs` está en `.gitignore`. Créalo desde el template:

```bash
cp ProyectoFinal/Config/AppConfig.cs.template ProyectoFinal/Config/AppConfig.cs
```

Llena los 4 valores:

```csharp
// Supabase — Settings > API en el dashboard
SupabaseUrl     = "https://xxxx.supabase.co"
SupabaseAnonKey = "eyJ..."

// Resend — API Keys > Create API Key
ResendApiKey = "re_xxxxxxxxxxxx"
EmailFrom    = "Pura Vida Yachts <onboarding@resend.dev>"
```

> `onboarding@resend.dev` funciona sin configurar dominio propio — útil para desarrollo y demos.

### 3. Ejecutar

**Visual Studio:** abre `ProyectoFinal.slnx`, selecciona plataforma, F5.

**CLI:**

```bash
dotnet build -t:Run -f net10.0-android
dotnet build -t:Run -f net10.0-ios
dotnet build -t:Run -f net10.0-maccatalyst
dotnet build -t:Run -f net10.0-windows10.0.19041.0
```

## Navegación

```
LoginPage ←→ RegistroPage
    ↓
MenuPage
    ├── CatalogoProductosPage → RegistrarCompraPage
    ├── GestionComprasPage → DetalleCompraPage
    │                            ├── RealizarPagoDirectoPage (mensual / extraordinario)
    │                            ├── ProgramarPagoPage
    │                            └── HistorialAbonosPage
    ├── AbonosPage          — historial consolidado de pagos
    ├── ConsultasPage       — saldo, abonos y fecha estimada por compra
    ├── ReportePage         — resumen de todas las compras activas
    ├── NotificacionesPage  — configurar email primario / secundario
    └── AyudaPage
```
