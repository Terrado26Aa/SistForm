# Changelog

Todas las versiones de SistForm documentadas en orden cronologico.

## [v1.0] - Fundamentos

Estructura base del proyecto con autenticacion (Login, Signin, HomePage), backend ASP.NET Core con controlador de autenticacion y modelos User, LoginRequestDto, RegisterRequestDto.

## [v1.1] - Servicios y API

Agregados modelos frontend y backend para registro de usuarios. Agregado servicio ApiService. Agregadas vistas CreateForm, PropertiesPanelView, Surveys. Configuracion de plataforma Android.

## [v1.2] - Toolbar Popup

Agregado PropertiesToolBarPopup. Modificaciones en CreateForm.xaml y appsettings.json.

## [v1.3] - Configuracion Maui

Modificacion en MauiProgram.cs (configuracion de la aplicacion).

## [v1.4] - Ajustes Proyecto

Ajustes en configuracion del proyecto Forms.csproj.

## [v1.5] - Toolbar y Eliminacion

Agregado ToolbarItem. Agregado delete_icon.png. Eliminados PropertiesPanelView y PropertiesToolBarPopup (reemplazados por toolbar).

## [v1.6] - Iconos y Mejoras

Agregados iconos: checklist_icon, delete_icon2, image_icon, plus_icon, question_icon, text_icon. Modificaciones en CreateForm y ToolbarItem.

## [v1.7] - Mejoras CreateForm

Mejoras en la logica de CreateForm.

## [v1.8] - Ajustes CreateForm

Ajustes adicionales en CreateForm.xaml.cs.

## [v1.9] - MainPage y Refactor

Agregados MainPage.xaml y MainPage.xaml.cs. Modificaciones en CreateForm, HomePage, Login. Actualizaciones en DbContext, controladores y ApiService.

## [v2.0] - Plataforma Android

Modificaciones en plataforma Android (manifest, actividades, aplicaciones).

## [v2.1] - Reestructuracion

Ajustes en archivos de proyecto (AuthLogin.csproj, Forms.sln, Forms.csproj).

## [v2.2] - Formularios y Controlador

Agregado FormsController. Agregados modelos FormDto, CForm, CreateFormDto. Modificaciones en estilos, fuentes, configuracion de app. Mejoras en vistas CreateForm, HomePage, Login.

## [v2.3] - Base de Datos

Modificacion en ApplicationDbContext.

## [v2.4] - Elementos de Formulario

Agregado modelo CFormElement. Modificaciones en CreateForm y Login. Actualizacion en AppShell.

## [v2.5] - Respuestas y Encuestas

Agregada FillSurveyPage para llenar encuestas. Agregados modelos ResponseDtos y CFormResponse.

## [v2.6] - Ajustes Proyecto

Ajustes en configuracion del proyecto.

## [v2.7] - Controlador Forms

Modificaciones en FormsController.

## [v2.8] - Vistas Encuestas

Ajustes en vistas FillSurveyPage.xaml y Surveys.xaml.

## [v2.9] - Logica Encuestas

Mejoras en logica de FillSurveyPage y Surveys.

## [v3.0] - Administracion

Agregada ManageFormsPage para administrar formularios. Modificaciones en FormsController, FillSurveyPage, CreateForm, Surveys.

## [v3.1] - Navegacion

Modificaciones en AppShell (navegacion).

## [v3.2] - Edicion de Formularios

Agregada EditFormPage para editar formularios existentes. Modificaciones en FormsController, CreateForm, ManageFormsPage.

## [v3.3] - Iconos y Gestion

Agregados iconos delete_icon3 y edit_icon. Mejoras en ManageFormsPage.

## [v3.4] - Perfil de Usuario

Agregado modelo UserProfileDto (frontend y backend).

## [v3.5] - Editar Perfil

Agregada EditProfilePage para editar perfil de usuario. Modificaciones en ApiService y controlador de autenticacion.

## [v3.6] - Ajustes Navegacion

Ajustes en AppShell (navegacion).

## [v3.7] - Soporte Offline

Agregada OfflineSurveysPage y servicio LocalDatabaseHelper. Soporte offline para encuestas.

## [v3.8] - Base Datos Local

Mejoras en LocalDatabaseHelper.
