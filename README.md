# Pura Vida Yatchs

**Sistema de gestión financiera para embarcaciones de alto valor**

Aplicación móvil desarrollada con .NET MAUI para el control de compras, abonos y reportes de embarcaciones.

## 🚢 Características

- Gestión de Compras de embarcaciones
- Registro de Abonos y pagos
- Consultas de historial de transacciones
- Reportes financieros
- Sistema de notificaciones

## 🛠️ Tecnologías

- .NET 10
- .NET MAUI
- C# 14.0

## 📋 Requisitos

- [Visual Studio 2022](https://visualstudio.microsoft.com/) (v17.13+)
- Carga de trabajo: **.NET Multi-platform App UI development**
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## 🚀 Instalación

```bash
git clone https://github.com/bradguillen15/EmbarcacionesAltoValor.git
cd EmbarcacionesAltoValor
dotnet restore
dotnet build
```

## ⚙️ Configuración de Supabase

Antes de ejecutar la aplicación, necesitas configurar las credenciales de Supabase:

1. Copia el archivo `ProyectoFinal/Config/AppConfig.cs.template` y renómbralo a `AppConfig.cs`
2. Edita `AppConfig.cs` con tus credenciales de Supabase:
   - `SupabaseUrl`: URL de tu proyecto en Supabase
   - `SupabaseAnonKey`: Anon/Public key de tu proyecto

3. **IMPORTANTE**: El archivo `AppConfig.cs` está en `.gitignore` y NO debe ser comiteado al repositorio.

### Obtener las credenciales de Supabase

1. Ve a tu proyecto en [Supabase](https://supabase.com)
2. Ve a Settings > API
3. Copia el **Project URL** y el **anon/public key**

### Seguridad

- ❌ No comitees `AppConfig.cs` con credenciales reales
- ✅ Usa el archivo `AppConfig.template.cs` como plantilla
- 🔐 En producción, considera usar variables de entorno o Azure Key Vault

## ▶️ Ejecutar

1. Abre `ProyectoFinal.sln` en Visual Studio
2. Configura Supabase (ver sección anterior)
3. Selecciona plataforma (Android/iOS/Windows)
4. Presiona **F5**

