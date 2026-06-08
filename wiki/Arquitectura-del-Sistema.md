# Arquitectura del Sistema

SistForm sigue una arquitectura **cliente-servidor** con un frontend multiplataforma en **.NET MAUI** y un backend **ASP.NET Core Web API**.

---

## Diagrama de Capas

```
┌─────────────────────────────────────────────────────────┐
│                    .NET MAUI (Frontend)                   │
│  ┌───────────┐  ┌──────────┐  ┌──────────────────────┐  │
│  │  Views     │  │ViewModels│  │     Services         │  │
│  │ (XAML)     │─▶│(MVVM)    │─▶│  ApiService          │  │
│  │            │  │          │  │  LocalDatabaseHelper  │  │
│  └───────────┘  └──────────┘  └──────────┬───────────┘  │
│                                           │              │
│  ┌────────────────────────────────────────┘              │
│  │  HTTP (JSON) + JWT Bearer Token                      │
│  └────────────────────────────────────────┐              │
└────────────────────────────────────────────│──────────────┘
                                             │
┌────────────────────────────────────────────│──────────────┐
│                    ASP.NET Core (Backend)   ▼              │
│  ┌──────────┐  ┌──────────┐  ┌──────────────────────┐  │
│  │Controllers│─▶│ Services │─▶│    Data Layer        │  │
│  │           │  │          │  │  ApplicationDbContext │  │
│  │ Auth      │  │ Auth     │  │  MySQL (EF Core)     │  │
│  │ Forms     │  │ Forms    │  │                      │  │
│  └──────────┘  └──────────┘  └──────────────────────┘  │
│                                                         │
│  ┌──────────────────────────────────────────────────┐   │
│  │ Middleware: GlobalExceptionMiddleware, JWT Auth,  │   │
│  │             Serilog, Swagger                      │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

---

## Frontend (.NET MAUI)

### Patrón MVVM
- **Views:** Páginas XAML con lógica de UI mínima en code-behind
- **ViewModels:** Solo `LoginViewModel` registrado actualmente
- **Services:**
  - `ApiService` — Cliente HTTP singleton para toda comunicación con el backend
  - `LocalDatabaseHelper` — Almacenamiento local offline con archivos JSON

### Comunicación
- HTTP/HTTPS con JSON
- Autenticación mediante JWT Bearer Token (almacenado en `SecureStorage` en iOS/Android, `Preferences` como fallback)
- URL base configurable en `Config.cs` según la plataforma:
  - Android emulador: `http://10.0.2.2:5174`
  - Otras plataformas: `http://127.0.0.1:5174`

### Plataformas Soportadas
| Plataforma | TFM |
|------------|-----|
| Android | `net10.0-android` |
| iOS | `net10.0-ios` |
| macOS (MacCatalyst) | `net10.0-maccatalyst` |
| Windows | `net10.0-windows10.0.19041.0` |

---

## Backend (ASP.NET Core)

### Capa de Controladores
- `AuthController` — Registro, login, perfil, administración de usuarios
- `FormsController` — CRUD de formularios, asignación, respuestas, dashboard

### Capa de Servicios
- `AuthService` — Lógica de autenticación y gestión de usuarios
- `FormsService` — Lógica de formularios, respuestas y asignaciones

### Capa de Datos
- `ApplicationDbContext` — Contexto de Entity Framework Core
- MySQL 8.0 como base de datos principal (producción)
- SQLite local para respaldo offline (a través de `LocalDatabaseHelper`)

### Middleware Pipeline
```
Request → Serilog Logging → GlobalExceptionMiddleware → Swagger (Dev) →
HTTPS Redirection → Authentication → Authorization → Controllers
```

### Autenticación JWT
- Claims: `NameIdentifier` (UserId), `Name` (UserName), `Email`, `Role` (User/Admin)
- Expiración: 8 horas
- Algoritmo: HMAC-SHA256
- Configuración en `appsettings.json`: `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`

---

## Flujo de Datos (Ejemplo: Login)

```
Login.xaml → LoginViewModel → ApiService.LoginAsync()
     │                              │
     │                              ▼
     │                    POST /api/auth/login
     │                         { UserName, Password }
     │                              │
     │                              ▼
     │                    AuthController.Login()
     │                              │
     │                              ▼
     │                    AuthService.LoginAsync()
     │                         BCrypt.Verify()
     │                         GenerateTokenJWT()
     │                              │
     │                              ▼
     │                    { Token, UserId, Role }
     │                              │
     │                              ▼
     │                    SecureStorage.SetAsync("token", token)
     │                    Preferences.Set("userId", userId)
     │                    Preferences.Set("role", role)
     │                              │
     │                              ▼
     │                    Navegación a HomePage
```

---

## Dependencias Principales

### Frontend (Forms.csproj)
| Paquete | Versión | Propósito |
|---------|---------|-----------|
| `Microsoft.Maui.Controls` | 10.0.60 | Framework MAUI |
| `CommunityToolkit.Maui` | 14.1.1 | Popups, behaviours, converters |
| `CommunityToolkit.Mvvm` | 8.4.2 | MVVM source generators |
| `Microsoft.Maui.Controls.Maps` | 10.0.60 | Mapas y geolocalización |
| `Newtonsoft.Json` | 13.0.4 | Serialización JSON |

### Backend (AuthLogin.csproj)
| Paquete | Versión | Propósito |
|---------|---------|-----------|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.6 | Autenticación JWT |
| `Microting.EntityFrameworkCore.MySql` | 10.0.5 | MySQL EF Core provider |
| `Swashbuckle.AspNetCore` | 10.1.7 | Swagger/OpenAPI |
| `Serilog.AspNetCore` | 10.0.0 | Logging estructurado |
| `BCrypt.Net-Next` | 4.0.3 | Hash de contraseñas |
| `GeoJSON.Net` | 1.4.1 | Formato GeoJSON para rutas |
