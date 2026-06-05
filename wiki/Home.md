# SistForm

Sistema de Encuestas (SistForm) es un sistema completo de creacion, gestion y respuesta de encuestas con soporte offline y sincronizacion, desarrollado en **.NET MAUI** (frontend) y **ASP.NET Core** (backend).

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
| [v2.6](v2-6-Ajustes-Proyecto) | Ajustes Proyecto | Ajustes en configuracion del proyecto. |
| [v2.7](v2-7-Controlador-Forms) | Controlador Forms | Modificaciones en FormsController. |
| [v2.8](v2-8-Vistas-Encuestas) | Vistas Encuestas | Ajustes en vistas FillSurveyPage.xaml y Surveys.xaml. |
| [v2.9](v2-9-Logica-Encuestas) | Logica Encuestas | Mejoras en logica de FillSurveyPage y Surveys. |
| [v3.0](v3-0-Administracion) | Administracion | Agregada ManageFormsPage para administrar formularios. Modificaciones en FormsController, FillSurveyPage, CreateForm, Surveys. |
| [v3.1](v3-1-Navegacion) | Navegacion | Modificaciones en AppShell (navegacion). |
| [v3.2](v3-2-Edicion-de-Formularios) | Edicion de Formularios | Agregada EditFormPage para editar formularios existentes. Modificaciones en FormsController, CreateForm, ManageFormsPage. |
| [v3.3](v3-3-Iconos-y-Gestion) | Iconos y Gestion | Agregados iconos delete_icon3 y edit_icon. Mejoras en ManageFormsPage. |
| [v3.4](v3-4-Perfil-de-Usuario) | Perfil de Usuario | Agregado modelo UserProfileDto (frontend y backend). |
| [v3.5](v3-5-Editar-Perfil) | Editar Perfil | Agregada EditProfilePage para editar perfil de usuario. Modificaciones en ApiService y controlador de autenticacion. |
| [v3.6](v3-6-Ajustes-Navegacion) | Ajustes Navegacion | Ajustes en AppShell (navegacion). |
| [v3.7](v3-7-Soporte-Offline) | Soporte Offline | Agregada OfflineSurveysPage y servicio LocalDatabaseHelper. Soporte offline para encuestas. |
| [v3.8](v3-8-Base-Datos-Local) | Base Datos Local | Mejoras en LocalDatabaseHelper. |
| [v3.9](v3-9-Sincronizacion) | Sincronizacion | Agregada SyncSurveysPage para sincronizacion de encuestas. Agregado upload_icon.png. |
| [v4.0](v4-0-Coordenadas-y-Limpieza) | Coordenadas y Limpieza | Agregadas migraciones de base de datos para coordenadas. Eliminado ToolbarItem (modelo obsoleto). |
| [v4.1](v4-1-Gestion-y-Tests) | Gestion y Tests | Agregados servicios backend AuthService y FormsService. Agregadas AssignFormPage, ResultsDashboardPage, UserManagementPage. Agregado GlobalExceptionMiddleware. Agregados tests unitarios. Agregado script de base de datos. |

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
| [Estados de Alfas](SEMAT-Alpha-States) | Matriz de progresion de alfas por cada version (v1.0 — v4.1) |
| [Espacios de Actividad](SEMAT-Activity-Spaces) | Mapa de espacios de actividad cubiertos en cada fase del proyecto |

---

## Enlaces

- [Repositorio](https://github.com/Terrado26Aa/SistForm)
- [Releases](https://github.com/Terrado26Aa/SistForm/releases)
- [Changelog](Changelog)