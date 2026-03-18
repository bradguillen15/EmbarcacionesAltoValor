# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**EmbarcacionesAltoValor** is a .NET MAUI cross-platform mobile app for luxury yacht financial management ("Pura Vida Yatchs"). It targets Android, iOS, macOS Catalyst, and Windows.

## Build & Run Commands

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run for a specific platform (from CLI)
dotnet build -t:Run -f net10.0-android
dotnet build -t:Run -f net10.0-ios
dotnet build -t:Run -f net10.0-maccatalyst
dotnet build -t:Run -f net10.0-windows10.0.19041.0
```

Preferred development environment: **Visual Studio 2022 (v17.13+)** — open `ProyectoFinal.slnx`, select target platform, press F5.

There are no test projects in this repository.

## Supabase Configuration (Required Before Running)

The file `ProyectoFinal/Config/AppConfig.cs` is gitignored. Copy the template and fill in your credentials:

```bash
cp ProyectoFinal/Config/AppConfig.cs.template ProyectoFinal/Config/AppConfig.cs
```

Then populate `AppConfig.cs` with the Supabase URL and anon key. **Never commit this file.**

## Architecture

### Navigation Flow

```
App.xaml.cs  →  LoginPage  →  MenuPage  →  (feature pages)
```

- `App.xaml.cs` sets `MainPage = new NavigationPage(new LoginPage())`
- `LoginPage` calls `AuthService.LoginAsync()`, stores `UserId`/`UserName` in `Preferences`, then pushes `MenuPage`
- `MenuPage` has 6 navigation buttons + logout (pops to root, clears preferences)

### Feature Pages (all stubs — no backend logic yet)

- `GestionComprasPage` — purchase management
- `AbonosPage` — payment registration
- `ConsultasPage` — data queries/lookups
- `ReportePage` — financial reports
- `NotificacionesPage` — email notifications
- `AyudaPage` — help

### Backend Integration

- **Supabase** (PostgreSQL via REST)
- `AuthService` POSTs to `/rest/v1/rpc/login` with JSON body `{ email, password }`
- Returns `LoginResponse { Id, Email, Nombre }`
- Uses Bearer token auth (`SupabaseAnonKey`) via `HttpClient`

### User Session

`Preferences.Set("UserId", ...)` and `Preferences.Set("UserName", ...)` are set after login and cleared on logout. No other state management library is used.

### Theming

Nautical color palette defined in `Resources/Styles/Colors.xaml`:
- Primary: `#0277BD` (ocean blue)
- Secondary: `#4FC3F7`
- Accent: `#00BCD4`
