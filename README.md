# Forms 1.0

Este proyecto es una solución en .NET que incluye una aplicación frontend desarrollada en **.NET MAUI** y una **API REST** backend desarrollada en **ASP.NET Core**. El sistema proporciona funcionalidades básicas de autenticación de usuarios (Inicio de sesión y Registro).

## Estructura del Proyecto

La solución `Forms.sln` contiene dos proyectos principales:

- **`AuthLogin/` (Backend):** Una Web API de ASP.NET Core que gestiona la autenticación de usuarios y la conexión a la base de datos MySQL mediante Entity Framework Core.
- **`Forms/` (Frontend):** Una aplicación multiplataforma de .NET MAUI con vistas para el inicio de sesión (`Login.xaml`), registro (`Signin.xaml`), y la página principal (`HomePage.xaml`).

## Requisitos Previos

Para ejecutar este proyecto, necesitarás instalar lo siguiente:

- [.NET SDK](https://dotnet.microsoft.com/download) (Compatible con .NET MAUI y ASP.NET Core, típicamente .NET 8 o superior).
- Un servidor de base de datos **MySQL**.
- Visual Studio 2022 (con cargas de trabajo de desarrollo de MAUI y ASP.NET) o VS Code con el C# Dev Kit.
- Herramientas de Entity Framework Core CLI (opcional, para migraciones: `dotnet tool install --global dotnet-ef`).

## Configuración y Ejecución

Sigue estos pasos para configurar y ejecutar el proyecto localmente.

### 1. Configuración de la Base de Datos

1. Asegúrate de que tu servidor MySQL esté ejecutándose.
2. Abre el archivo de configuración del backend: `AuthLogin/appsettings.json`.
3. Modifica la cadena de conexión (`DefaultConnection`) para que coincida con tus credenciales locales de MySQL:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=dbforms;Uid=TU_USUARIO;Pwd=TU_CONTRASEÑA;"
   }
   ```
4. Abre una terminal en la carpeta `AuthLogin` y aplica las migraciones para crear la base de datos:
   ```bash
   cd AuthLogin
   dotnet ef database update
   ```

### 2. Ejecutar la API Backend

1. Desde la terminal en la carpeta `AuthLogin`, inicia el servidor:
   ```bash
   dotnet run
   ```
2. La API se ejecutará (típicamente en un puerto como `https://localhost:5001` o `http://localhost:5000`). Mantén esta terminal abierta.

### 3. Ejecutar la Aplicación MAUI (Frontend)

1. Abre la solución `Forms.sln` en Visual Studio.
2. Asegúrate de configurar la URL de la API en el código de la aplicación MAUI (normalmente en los servicios o en las vistas como `Login.xaml.cs`) para que apunte a la dirección local donde se está ejecutando `AuthLogin`. **Nota:** Si pruebas en un emulador de Android, `localhost` se refiere al emulador mismo; usa `10.0.2.2` para referirte a tu máquina local.
3. Selecciona tu dispositivo de destino (Ej. Emulador de Android, Windows Machine) como proyecto de inicio.
4. Presiona **F5** o haz clic en "Ejecutar".

## Tecnologías Utilizadas

- **Frontend:** .NET MAUI, XAML, C#
- **Backend:** ASP.NET Core Web API, C#
- **Base de datos:** MySQL, Entity Framework Core
