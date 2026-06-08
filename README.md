# SistForm

Sistema de Encuestas movil con soporte offline, sincronizacion y captura de coordenadas geograficas.

Desarrollado en **.NET MAUI** (frontend) y **ASP.NET Core** (backend) como Trabajo de Graduacion.

## Funcionalidades

- **Autenticacion** — Login, registro y gestion de perfiles de usuario
- **Formularios** — Creacion, edicion, asignacion y administracion de encuestas
- **Encuestas** — Respuesta a formularios desde el dispositivo movil
- **Soporte Offline** — Almacenamiento local con SQLite y sincronizacion automatica
- **Geolocalizacion** — Captura de coordenadas geograficas en las respuestas
- **Administracion** — Panel de resultados, gestion de usuarios y dashboard
- **Multiplataforma** — Android, iOS, Windows y Mac Catalyst

## Arquitectura

```
SistForm/
├── AuthLogin/                  # Backend ASP.NET Core Web API
│   ├── Controllers/            # AuthController, FormsController
│   ├── Services/               # AuthService, FormsService
│   ├── Models/                 # CForm, User, DTOs
│   ├── Data/                   # ApplicationDbContext (EF Core)
│   ├── Middleware/             # GlobalExceptionMiddleware
│   └── Migrations/             # Migraciones de base de datos
├── Forms/                      # Frontend .NET MAUI
│   ├── Views/                  # Paginas XAML (login, encuestas, admin)
│   ├── ViewModels/             # LoginViewModel
│   ├── Services/               # ApiService, LocalDatabaseHelper
│   ├── Models/                 # DTOs del frontend
│   ├── Converters/             # RoleToColorConverter
│   └── Platforms/              # Android, iOS, Windows, Mac, Tizen
├── AuthLogin.Tests/            # Pruebas unitarias (xUnit + Moq)
└── Base de Datos/              # Script SQL de la base de datos
```

## Tecnologias

| Capa | Tecnologia |
|------|-----------|
| Frontend | .NET MAUI (C#, XAML) |
| Backend | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Base de datos | MySQL |
| Almacenamiento local | SQLite |
| Testing | xUnit + Moq |
| Metodologia | SEMAT / ESSENCE |

## Requisitos Previos

- [.NET SDK](https://dotnet.microsoft.com/download) 10.0 o superior
- **MySQL** server
- Visual Studio 2022 (cargas de trabajo MAUI y ASP.NET) o VS Code + C# Dev Kit
- Entity Framework Core CLI (opcional): `dotnet tool install --global dotnet-ef`

## Configuracion y Ejecucion

### Backend

```bash
cd AuthLogin
# Editar appsettings.json con cadena de conexion MySQL
dotnet run
```

La API se ejecuta en `https://localhost:5001` o `http://localhost:5000`.

### Base de Datos

1. Asegurate de que MySQL este ejecutandose
2. Configura la cadena de conexion en `AuthLogin/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=dbforms;Uid=TU_USUARIO;Pwd=TU_CONTRASENA;"
   }
   ```
3. Aplica las migraciones:
   ```bash
   cd AuthLogin
   dotnet ef database update
   ```

### Frontend (MAUI)

1. Abre `Forms.sln` en Visual Studio
2. Configura la URL de la API en `Forms/Services/ApiService.cs`
3. Selecciona el destino (Android Emulator, Windows, etc.)
4. Presiona **F5** o haz clic en "Ejecutar"

> En un emulador de Android, usa `10.0.2.2` en lugar de `localhost`.

### Pruebas

```bash
dotnet test AuthLogin.Tests/AuthLogin.Tests.csproj
```

## API Endpoints

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | `/api/auth/login` | Iniciar sesion |
| POST | `/api/auth/register` | Registrar usuario |
| GET | `/api/forms` | Listar formularios |
| POST | `/api/forms` | Crear formulario |
| PUT | `/api/forms/{id}` | Editar formulario |
| DELETE | `/api/forms/{id}` | Eliminar formulario |
| POST | `/api/forms/{id}/respond` | Responder encuesta |
| GET | `/api/forms/{id}/results` | Resultados de encuesta |
| GET | `/api/admin/users` | Gestionar usuarios (admin) |

## Metodologia SEMAT / ESSENCE

El proyecto se rige bajo **SEMAT (Software Engineering Method and Theory)** con el kernel **ESSENCE**, utilizando 7 alfas para medir progreso y salud del proyecto:

| Alfa | Proposito |
|------|-----------|
| **Opportunity** | Necesidad que motiva el sistema |
| **Stakeholders** | Personas afectadas por el sistema |
| **Requirements** | Capacidades que el sistema debe proveer |
| **Software System** | El sistema de software en construccion |
| **Work** | Actividades para construir el sistema |
| **Team** | Equipo responsable del desarrollo |
| **Way of Working** | Metodologia y herramientas utilizadas |

> Documentacion completa de SEMAT en la [wiki](https://github.com/Terrado26Aa/SistForm/wiki).

## Versionado

| Version | Nombre | Descripcion |
|---------|--------|-------------|
| v1.0 | Fundamentos | Estructura base con autenticacion |
| v1.1 | Servicios y API | ApiService, vistas CreateForm/Surveys |
| v1.2 | Toolbar Popup | PropertiesToolBarPopup |
| v1.3 | Configuracion Maui | MauiProgram.cs |
| v1.4 | Ajustes Proyecto | Config Forms.csproj |
| v1.5 | Toolbar y Eliminacion | ToolbarItem, iconos |
| v1.6 | Iconos y Mejoras | Iconos, mejoras CreateForm |
| v1.7 | Mejoras CreateForm | Logica CreateForm |
| v1.8 | Ajustes CreateForm | Ajustes CreateForm.xaml.cs |
| v1.9 | MainPage y Refactor | MainPage, refactor general |
| v2.0 | Plataforma Android | Configuracion Android |
| v2.1 | Reestructuracion | Ajustes archivos proyecto |
| v2.2 | Formularios | FormsController, modelos |
| v2.3 | Base de Datos | ApplicationDbContext |
| v2.4 | Elementos | CFormElement |
| v2.5 | Respuestas | FillSurveyPage, ResponseDtos |
| v2.6 | Ajustes Proyecto | Config Forms.csproj |
| v2.7 | Controlador Forms | FormsController |
| v2.8 | Vistas Encuestas | FillSurveyPage, Surveys |
| v2.9 | Logica Encuestas | Mejoras FillSurveyPage |
| v3.0 | Administracion | ManageFormsPage |
| v3.1 | Navegacion | AppShell |
| v3.2 | Edicion Formularios | EditFormPage |
| v3.3 | Iconos y Gestion | Iconos, mejoras manage |
| v3.4 | Perfil Usuario | UserProfileDto |
| v3.5 | Editar Perfil | EditProfilePage |
| v3.6 | Ajustes Navegacion | AppShell |
| v3.7 | Soporte Offline | OfflineSurveysPage, LocalDB |
| v3.8 | Base Datos Local | Mejoras LocalDatabaseHelper |
| v3.9 | Sincronizacion | SyncSurveysPage |
| v4.0 | Coordenadas | Geolocalizacion, limpieza |
| v4.1 | Gestion y Tests | AuthService, FormsService, tests unitarios |
| v4.2 | .NET 10 y CI/CD | Migracion a .NET 10, fix pipeline multiplataforma |

> Documentacion detallada de cada version en la [wiki](https://github.com/Terrado26Aa/SistForm/wiki).

## Licencia

Este proyecto es un Trabajo de Graduacion. Todos los derechos reservados.

## Enlaces

- [Repositorio](https://github.com/Terrado26Aa/SistForm)
- [Wiki](https://github.com/Terrado26Aa/SistForm/wiki)
- [Changelog](https://github.com/Terrado26Aa/SistForm/wiki/Changelog)
- [Reportar vulnerabilidad](SECURITY.md)
