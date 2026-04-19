# Pura Vida Yachts

**Sistema de gestión financiera para embarcaciones de alto valor**

Aplicación móvil desarrollada con .NET MAUI que permite a un cliente registrar, administrar y consultar sus compras financiadas de embarcaciones recreativas (jet ski, lanchas). Incluye cálculo real de amortización, control de abonos y notificaciones automáticas por correo.

## Módulos

| Módulo | Estado | Descripción |
|--------|--------|-------------|
| Login / Registro | ✅ | Autenticación con Supabase, sesión persistente |
| Catálogo de productos | ✅ | Lista de embarcaciones disponibles con precios |
| Gestión de compras | ✅ | Registro de compras con calculadora de amortización |
| Abonos | ✅ | Historial consolidado de todos los pagos |
| Consultas financieras | ✅ | Saldo, abonos realizados y fecha estimada de liquidación |
| Reportes | ✅ | Resumen consolidado de todas las compras activas |
| Notificaciones por correo | ✅ | Envío automático vía SMTP al registrar compras y abonos |
| Ayuda | ✅ | Contacto con soporte |

## Cálculo de amortización

Cada compra aplica la fórmula estándar de amortización francesa:

```
TasaMensual  = TasaAnual / 12 / 100
CuotaMensual = MontoFinanciado × TasaMensual / (1 − (1 + TasaMensual)^(−PlazoMeses))
TotalAPagar  = CuotaMensual × PlazoMeses
```

El `TotalConIntereses` se guarda en la tabla `compras` al momento del registro (valor contractual). El `SaldoPendiente` se calcula en la vista `vw_compra_con_saldo` como `TotalConIntereses − SUM(abonos)`.

- **Pago mensual:** se pre-llena con la cuota mensual calculada
- **Pago extraordinario:** monto libre, descuenta directo del saldo

## Tecnologías

- .NET 10 / .NET MAUI — C# 14
- Supabase (PostgreSQL + REST API)
- MailKit — envío de notificaciones SMTP
- Plataformas: Android, iOS, macOS Catalyst, Windows

## Requisitos

- [Visual Studio 2022](https://visualstudio.microsoft.com/) v17.13+ con la carga de trabajo **.NET Multi-platform App UI development**
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Proyecto activo en [Supabase](https://supabase.com)
- Cuenta Gmail con [App Password](https://myaccount.google.com/apppasswords) habilitada (para notificaciones)

## Instalación

```bash
git clone https://github.com/bradguillen15/EmbarcacionesAltoValor.git
cd EmbarcacionesAltoValor
dotnet restore
dotnet build
```

## Configuración

### 1. AppConfig (Supabase + SMTP)

```bash
cp ProyectoFinal/Config/AppConfig.cs.template ProyectoFinal/Config/AppConfig.cs
```

Edita `AppConfig.cs` con tus credenciales:

```csharp
// Supabase
SupabaseUrl     = "https://xxxx.supabase.co"
SupabaseAnonKey = "eyJ..."

// Gmail SMTP
SmtpUser     = "tucorreo@gmail.com"
SmtpPassword = "app-password-de-16-caracteres"
```

> `AppConfig.cs` está en `.gitignore` y **no debe comitearse**.

### 2. Migración de base de datos

Ejecuta el archivo [`supabase_migration_intereses.sql`](supabase_migration_intereses.sql) en **Supabase Dashboard → SQL Editor**. Este script:

- Agrega la columna `TotalConIntereses` a la tabla `compras`
- Recalcula el valor para compras existentes
- Actualiza la vista `vw_compra_con_saldo` para usar ese campo en el cálculo del saldo pendiente

### 3. Gmail App Password

1. Ve a [myaccount.google.com](https://myaccount.google.com) → Seguridad → Verificación en dos pasos
2. Al final de esa página → **Contraseñas de aplicación**
3. Genera una para "Correo" y cópiala en `AppConfig.SmtpPassword`

## Ejecutar

**Visual Studio (recomendado):**

1. Abre `ProyectoFinal.slnx`
2. Selecciona plataforma (Android / iOS / Windows)
3. Presiona **F5**

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
    │                            ├── RealizarPagoDirectoPage
    │                            ├── ProgramarPagoPage
    │                            └── HistorialAbonosPage
    ├── AbonosPage          (historial consolidado de pagos)
    ├── ConsultasPage       (saldo, abonos, fecha estimada)
    ├── ReportePage         (resumen de compras activas)
    ├── NotificacionesPage  (configurar emails de notificación)
    └── AyudaPage
```

La sesión (`UserId`, `UserName`, `UserEmail`) se persiste con la API `Preferences` de MAUI y se limpia al cerrar sesión.
