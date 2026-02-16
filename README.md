# EmbarcacionesAltoValor

Sistema de gestión financiera para embarcaciones de alto valor. Aplicación móvil desarrollada con .NET MAUI para el control de compras, abonos y reportes.

## ?? Características

- **Gestión de Compras**: Registro y seguimiento de compras de embarcaciones
- **Registro de Abonos**: Control de pagos y abonos realizados
- **Consultas**: Historial completo de transacciones
- **Reportes Generales**: Visualización de estados financieros
- **Notificaciones**: Sistema de alertas por correo electrónico
- **Ayuda y Soporte**: Información de contacto y preguntas frecuentes

## ??? Tecnologías

- **.NET 10**
- **.NET MAUI** (Multi-platform App UI)
- **C# 14.0**
- **XAML** para diseño de interfaces

## ?? Requisitos Previos

Antes de ejecutar el proyecto, asegúrate de tener instalado:

### Windows
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (versión 17.13 o superior)
- Carga de trabajo: **.NET Multi-platform App UI development**
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### macOS
- [Visual Studio 2022 para Mac](https://visualstudio.microsoft.com/vs/mac/) o [Visual Studio Code](https://code.visualstudio.com/)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Xcode (para desarrollo iOS)

## ?? Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/EmbarcacionesAltoValor.git
cd EmbarcacionesAltoValor
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Compilar el proyecto

```bash
dotnet build
```

## ?? Ejecución

### Visual Studio 2022

1. Abre la solución `ProyectoFinal.sln` en Visual Studio
2. Selecciona la plataforma de destino (Android, iOS, Windows, macOS)
3. Presiona **F5** o haz clic en el botón **Run**

### Línea de comandos

#### Android
```bash
dotnet build -t:Run -f net10.0-android
```

#### iOS (requiere macOS)
```bash
dotnet build -t:Run -f net10.0-ios
```

#### Windows
```bash
dotnet build -t:Run -f net10.0-windows10.0.19041.0
```

#### macOS
```bash
dotnet build -t:Run -f net10.0-maccatalyst
```

## ?? Plataformas Soportadas

- ? **Android** 5.0 (API 21) o superior
- ? **iOS** 15.0 o superior
- ? **Windows** 10.0.17763.0 o superior
- ? **macOS** 15.0 o superior (Mac Catalyst)

## ??? Estructura del Proyecto

```
EmbarcacionesAltoValor/
??? ProyectoFinal/
?   ??? Views/              # Páginas XAML de la aplicación
?   ?   ??? LoginPage.xaml
?   ?   ??? MenuPage.xaml
?   ?   ??? RegistroPage.xaml
?   ?   ??? AyudaPage.xaml
?   ?   ??? ...
?   ??? Resources/          # Recursos (imágenes, estilos, fuentes)
?   ?   ??? Styles/
?   ?   ??? Images/
?   ?   ??? Fonts/
?   ??? AppShell.xaml       # Shell de navegación
?   ??? App.xaml            # Aplicación principal
?   ??? MauiProgram.cs      # Punto de entrada
??? README.md
```

## ?? Tema de Colores

La aplicación utiliza un tema náutico con escala de azules:

- **Primary**: `#0277BD` - Azul océano
- **Secondary**: `#4FC3F7` - Azul cielo
- **Tertiary**: `#006064` - Verde azulado profundo

## ?? Configuración Adicional

### Android

Para ejecutar en Android, necesitas:
- Android SDK instalado
- Emulador Android configurado o dispositivo físico conectado

### iOS

Para ejecutar en iOS, necesitas:
- macOS con Xcode instalado
- Certificados de desarrollo de Apple configurados
- Simulador iOS o dispositivo físico

### Windows

Para ejecutar en Windows:
- Windows 10 versión 1809 (build 17763) o superior
- Windows SDK instalado

## ?? Soporte

Si necesitas ayuda, contacta:

- **Email**: soporte@highvaluepay.com
- **Teléfono**: +506 0000-0000
- **Horario**: Lunes a Viernes, 8:00 AM - 6:00 PM

## ?? Licencia

Este proyecto es parte de un trabajo académico/privado.

## ?? Contribuciones

Las contribuciones son bienvenidas. Por favor:

1. Haz fork del proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## ?? Actualización de Dependencias

Para actualizar las dependencias de NuGet:

```bash
dotnet restore
dotnet list package --outdated
dotnet add package [PackageName] --version [Version]
```

## ?? Problemas Conocidos

- La navegación absoluta a rutas globales puede causar errores. Usar navegación relativa para páginas registradas con `Routing.RegisterRoute()`
- Las tildes en XAML deben usar entidades XML (`&#225;` para á) para compatibilidad multiplataforma

## ?? Estado del Proyecto

?? En desarrollo activo
