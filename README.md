# SistForm

Sistema de Encuestas (SistForm) es un sistema de encuestas con autenticacion de usuarios, desarrollado en **.NET MAUI** (frontend) y **ASP.NET Core** (backend).

## Funcionalidades

- Autenticacion de usuarios (Login, Registro)
- Creacion de formularios y encuestas
- Toolbar contextual para edicion de elementos

## Tecnologias

- **Frontend:** .NET MAUI (C#, XAML)
- **Backend:** ASP.NET Core Web API

## Requisitos Previos

- [.NET SDK](https://dotnet.microsoft.com/download) (.NET 8 o superior)
- Visual Studio 2022 (con cargas de trabajo MAUI y ASP.NET) o VS Code con C# Dev Kit

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

> *Documentacion completa disponible en la [wiki](wiki/Home).*

---

## Enlaces

- [Repositorio](https://github.com/Terrado26Aa/SistForm)
- [Wiki](wiki/Home)
- [Changelog](wiki/Changelog)