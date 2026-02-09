# SistForm

Sistema de Encuestas (SistForm) es un sistema de creacion, gestion y respuesta de encuestas, desarrollado en **.NET MAUI** (frontend) y **ASP.NET Core** (backend).

---

## Versiones

| Version | Nombre | Descripcion |
|---------|--------|-------------|
| [v1.0](v1-0-Fundamentos) | Fundamentos | Estructura base del proyecto con autenticacion (Login, Signin, HomePage), backend ASP.NET Core con controlador de autenticacion y modelos User, LoginRequestDto, RegisterRequestDto. |
| [v1.1](v1-1-Servicios-y-API) | Servicios y API | Agregados modelos frontend y backend para registro de usuarios. Agregado servicio ApiService. Agregadas vistas CreateForm, PropertiesPanelView, Surveys. Configuracion de plataforma Android. |
| [v1.2](v1-2-Toolbar-Popup) | Toolbar Popup | Agregado PropertiesToolBarPopup. Modificaciones en CreateForm.xaml y appsettings.json. |
| [v1.3](v1-3-Configuracion-Maui) | Configuracion Maui | Modificacion en MauiProgram.cs (configuracion de la aplicacion). |
| [v1.4](v1-4-Ajustes-Proyecto) | Ajustes Proyecto | Ajustes en configuracion del proyecto Forms.csproj. |
| [v1.5](v1-5-Toolbar-y-Eliminacion) | Toolbar y Eliminacion | Agregado ToolbarItem. Agregado delete_icon.png. Eliminados PropertiesPanelView y PropertiesToolBarPopup (reemplazados por toolbar). |
| [v1.6](v1-6-Iconos-y-Mejoras) | Iconos y Mejoras | Agregados iconos: checklist_icon, delete_icon2, image_icon, plus_icon, question_icon, text_icon. Modificaciones en CreateForm y ToolbarItem. |
| [v1.7](v1-7-Mejoras-CreateForm) | Mejoras CreateForm | Mejoras en la logica de CreateForm. |
| [v1.8](v1-8-Ajustes-CreateForm) | Ajustes CreateForm | Ajustes adicionales en CreateForm.xaml.cs. |
| [v1.9](v1-9-MainPage-y-Refactor) | MainPage y Refactor | Agregados MainPage.xaml y MainPage.xaml.cs. Modificaciones en CreateForm, HomePage, Login. Actualizaciones en DbContext, controladores y ApiService. |
| [v2.0](v2-0-Plataforma-Android) | Plataforma Android | Modificaciones en plataforma Android (manifest, actividades, aplicaciones). |
| [v2.1](v2-1-Reestructuracion) | Reestructuracion | Ajustes en archivos de proyecto (AuthLogin.csproj, Forms.sln, Forms.csproj). |
| [v2.2](v2-2-Formularios-y-Controlador) | Formularios y Controlador | Agregado FormsController. Agregados modelos FormDto, CForm, CreateFormDto. Modificaciones en estilos, fuentes, configuracion de app. Mejoras en vistas CreateForm, HomePage, Login. |
| [v2.3](v2-3-Base-de-Datos) | Base de Datos | Modificacion en ApplicationDbContext. |
| [v2.4](v2-4-Elementos-de-Formulario) | Elementos de Formulario | Agregado modelo CFormElement. Modificaciones en CreateForm y Login. Actualizacion en AppShell. |
| [v2.5](v2-5-Respuestas-y-Encuestas) | Respuestas y Encuestas | Agregada FillSurveyPage para llenar encuestas. Agregados modelos ResponseDtos y CFormResponse. |

---

## Documentacion del Anteproyecto

- [Introduccion y Objetivos](Anteproyecto-Introduccion) — Contexto, justificacion, objetivos general y especificos
- [Plan de Contenido](Plan-de-Contenido) — Estructura completa de la tesis
- [Marco Teorico](Marco-Teorico) — Antecedentes y trabajos relacionados
- [Alcance y Justificacion](Alcance-y-Justificacion) — Definicion del problema y alcance del proyecto

---

## SEMAT / ESSENCE

| Pagina | Descripcion |
|--------|-------------|
| [Kernel ESSENCE](SEMAT-Kernel) | Definicion de las 7 alfas con checklist de estados para SistForm |
| [Estados de Alfas](SEMAT-Alpha-States) | Matriz de progresion de alfas por cada version (v1.0 — v2.5) |
| [Espacios de Actividad](SEMAT-Activity-Spaces) | Mapa de espacios de actividad cubiertos en cada fase del proyecto |

---

## Enlaces

- [Repositorio](https://github.com/Terrado26Aa/SistForm)
- [Releases](https://github.com/Terrado26Aa/SistForm/releases)
- [Changelog](Changelog)