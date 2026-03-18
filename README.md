# Pura Vida Yatchs

**Sistema de gestión financiera para embarcaciones de alto valor**

Aplicación móvil desarrollada con .NET MAUI para el control de compras, abonos y reportes de embarcaciones.

## 🚢 Características

| Módulo | Estado |
|--------|--------|
| Autenticación (login con Supabase) | ✅ Implementado |
| Gestión de Compras | 🔧 En desarrollo |
| Registro de Abonos y pagos | 🔧 En desarrollo |
| Consultas de historial | 🔧 En desarrollo |
| Reportes financieros | 🔧 En desarrollo |
| Sistema de notificaciones por email | 🔧 En desarrollo |

## 🛠️ Tecnologías

- .NET 10 / .NET MAUI
- C# 14.0
- Supabase (PostgreSQL + REST API)
- Plataformas: Android, iOS, macOS Catalyst, Windows

## 📋 Requisitos

- [Visual Studio 2022](https://visualstudio.microsoft.com/) (v17.13+) con la carga de trabajo **.NET Multi-platform App UI development**
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Credenciales de un proyecto en [Supabase](https://supabase.com)

## 🚀 Instalación

```bash
git clone https://github.com/bradguillen15/EmbarcacionesAltoValor.git
cd EmbarcacionesAltoValor
dotnet restore
dotnet build
```

## ⚙️ Configuración de Supabase

Antes de ejecutar la aplicación, configura las credenciales de Supabase:

1. Copia la plantilla y renómbrala:
   ```bash
   cp ProyectoFinal/Config/AppConfig.cs.template ProyectoFinal/Config/AppConfig.cs
   ```
2. Edita `AppConfig.cs` con tus credenciales:
   - `SupabaseUrl`: URL de tu proyecto (Settings > API > Project URL)
   - `SupabaseAnonKey`: Anon/Public key (Settings > API > anon/public)

> **IMPORTANTE:** `AppConfig.cs` está en `.gitignore` y **no debe ser comiteado** al repositorio.

## ▶️ Ejecutar

**Visual Studio (recomendado):**

1. Abre `ProyectoFinal.slnx` en Visual Studio 2022
2. Configura Supabase (ver sección anterior)
3. Selecciona la plataforma (Android / iOS / Windows)
4. Presiona **F5**

**CLI por plataforma:**

```bash
dotnet build -t:Run -f net10.0-android
dotnet build -t:Run -f net10.0-ios
dotnet build -t:Run -f net10.0-maccatalyst
dotnet build -t:Run -f net10.0-windows10.0.19041.0
```

## 🗺️ Arquitectura de navegación

```
LoginPage  →  MenuPage  →  GestionComprasPage
                       →  AbonosPage
                       →  ConsultasPage
                       →  ReportePage
                       →  NotificacionesPage
                       →  AyudaPage
```

La sesión del usuario (`UserId`, `UserName`) se persiste con la API `Preferences` de MAUI y se limpia al cerrar sesión.
