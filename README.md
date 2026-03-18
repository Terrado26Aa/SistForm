# SistForm

Sistema de Encuestas (SistForm) es un sistema de creacion, gestion y respuesta de encuestas, desarrollado en **.NET MAUI** (frontend) y **ASP.NET Core** (backend).

## Funcionalidades

- Autenticacion de usuarios (Login, Registro)
- Creacion de formularios y encuestas
- Toolbar contextual para edicion de elementos
- Refactorizacion de vistas y navegacion
- Configuracion para plataforma Android
- API REST para formularios con Entity Framework Core y MySQL
- Respuesta a encuestas desde dispositivo movil
- Administracion de formularios (CRUD completo)
- Edicion de formularios existentes
- Perfiles de usuario y edicion

## Tecnologias

- **Frontend:** .NET MAUI (C#, XAML)
- **Backend:** ASP.NET Core Web API
- **ORM:** Entity Framework Core
- **Base de datos:** MySQL

## Requisitos Previos

- [.NET SDK](https://dotnet.microsoft.com/download) (.NET 8 o superior)
- Servidor **MySQL**
- Visual Studio 2022 (con cargas de trabajo MAUI y ASP.NET) o VS Code con C# Dev Kit
- Entity Framework Core CLI (opcional): `dotnet tool install --global dotnet-ef`

## Configuracion y Ejecucion

### 1. Backend

```bash
cd AuthLogin
dotnet run
```
La API se ejecuta en `https://localhost:5001` o `http://localhost:5000`.

### 2. Frontend (MAUI)

1. Abre `Forms.sln` en Visual Studio.
2. Configura la URL de la API en los servicios del frontend para que apunte al backend local.
3. Selecciona el dispositivo destino (Android Emulator, Windows Machine, etc.).
4. Presiona **F5** o haz clic en "Ejecutar".

> **Nota:** Si pruebas en un emulador de Android, usa `10.0.2.2` en lugar de `localhost` para referirte a la maquina local.

### 3. Base de Datos

1. Asegurate de que MySQL este ejecutandose.
2. Edita `AuthLogin/appsettings.json` y configura la cadena de conexion:
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

---

## Versiones

| Version | Nombre | Descripcion |
|---------|--------|-------------|
| [v1.0](wiki/v1-0-Fundamentos) | Fundamentos | Estructura base del proyecto con autenticacion (Login, Signin, HomePage), backend ASP.NET Core con controlador de autenticacion y modelos User, LoginRequestDto, RegisterRequestDto. |
| [v1.1](wiki/v1-1-Servicios-y-API) | Servicios y API | Agregados modelos frontend y backend para registro de usuarios. Agregado servicio ApiService. Agregadas vistas CreateForm, PropertiesPanelView, Surveys. Configuracion de plataforma Android. |
| [v1.2](wiki/v1-2-Toolbar-Popup) | Toolbar Popup | Agregado PropertiesToolBarPopup. Modificaciones en CreateForm.xaml y appsettings.json. |
| [v1.3](wiki/v1-3-Configuracion-Maui) | Configuracion Maui | Modificacion en MauiProgram.cs (configuracion de la aplicacion). |
| [v1.4](wiki/v1-4-Ajustes-Proyecto) | Ajustes Proyecto | Ajustes en configuracion del proyecto Forms.csproj. |
| [v1.5](wiki/v1-5-Toolbar-y-Eliminacion) | Toolbar y Eliminacion | Agregado ToolbarItem. Agregado delete_icon.png. Eliminados PropertiesPanelView y PropertiesToolBarPopup (reemplazados por toolbar). |
| [v1.6](wiki/v1-6-Iconos-y-Mejoras) | Iconos y Mejoras | Agregados iconos: checklist_icon, delete_icon2, image_icon, plus_icon, question_icon, text_icon. Modificaciones en CreateForm y ToolbarItem. |
| [v1.7](wiki/v1-7-Mejoras-CreateForm) | Mejoras CreateForm | Mejoras en la logica de CreateForm. |
| [v1.8](wiki/v1-8-Ajustes-CreateForm) | Ajustes CreateForm | Ajustes adicionales en CreateForm.xaml.cs. |
| [v1.9](wiki/v1-9-MainPage-y-Refactor) | MainPage y Refactor | Agregados MainPage.xaml y MainPage.xaml.cs. Modificaciones en CreateForm, HomePage, Login. Actualizaciones en DbContext, controladores y ApiService. |
| [v2.0](wiki/v2-0-Plataforma-Android) | Plataforma Android | Modificaciones en plataforma Android (manifest, actividades, aplicaciones). |
| [v2.1](wiki/v2-1-Reestructuracion) | Reestructuracion | Ajustes en archivos de proyecto (AuthLogin.csproj, Forms.sln, Forms.csproj). |
| [v2.2](wiki/v2-2-Formularios-y-Controlador) | Formularios y Controlador | Agregado FormsController. Agregados modelos FormDto, CForm, CreateFormDto. Modificaciones en estilos, fuentes, configuracion de app. Mejoras en vistas CreateForm, HomePage, Login. |
| [v2.3](wiki/v2-3-Base-de-Datos) | Base de Datos | Modificacion en ApplicationDbContext. |
| [v2.4](wiki/v2-4-Elementos-de-Formulario) | Elementos de Formulario | Agregado modelo CFormElement. Modificaciones en CreateForm y Login. Actualizacion en AppShell. |
| [v2.5](wiki/v2-5-Respuestas-y-Encuestas) | Respuestas y Encuestas | Agregada FillSurveyPage para llenar encuestas. Agregados modelos ResponseDtos y CFormResponse. |
| [v2.6](wiki/v2-6-Ajustes-Proyecto) | Ajustes Proyecto | Ajustes en configuracion del proyecto. |
| [v2.7](wiki/v2-7-Controlador-Forms) | Controlador Forms | Modificaciones en FormsController. |
| [v2.8](wiki/v2-8-Vistas-Encuestas) | Vistas Encuestas | Ajustes en vistas FillSurveyPage.xaml y Surveys.xaml. |
| [v2.9](wiki/v2-9-Logica-Encuestas) | Logica Encuestas | Mejoras en logica de FillSurveyPage y Surveys. |
| [v3.0](wiki/v3-0-Administracion) | Administracion | Agregada ManageFormsPage para administrar formularios. Modificaciones en FormsController, FillSurveyPage, CreateForm, Surveys. |
| [v3.1](wiki/v3-1-Navegacion) | Navegacion | Modificaciones en AppShell (navegacion). |
| [v3.2](wiki/v3-2-Edicion-de-Formularios) | Edicion de Formularios | Agregada EditFormPage para editar formularios existentes. Modificaciones en FormsController, CreateForm, ManageFormsPage. |
| [v3.3](wiki/v3-3-Iconos-y-Gestion) | Iconos y Gestion | Agregados iconos delete_icon3 y edit_icon. Mejoras en ManageFormsPage. |
| [v3.4](wiki/v3-4-Perfil-de-Usuario) | Perfil de Usuario | Agregado modelo UserProfileDto (frontend y backend). |

> *Documentacion completa disponible en la [wiki](wiki/Home).*

---

## Enlaces

- [Repositorio](https://github.com/Terrado26Aa/SistForm)
- [Wiki](wiki/Home)
- [Changelog](wiki/Changelog)