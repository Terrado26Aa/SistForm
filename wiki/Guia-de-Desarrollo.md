# Guía de Desarrollo

Guía para configurar el entorno de desarrollo y contribuir al proyecto SistForm.

---

## Requisitos

| Herramienta | Versión Mínima | Propósito |
|-------------|----------------|-----------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0.x | Compilación y ejecución |
| [Visual Studio 2022](https://visualstudio.microsoft.com/) | 17.14+ | IDE (Windows) |
| [VS Code](https://code.visualstudio.com/) | — | IDE alternativo |
| [MySQL Server](https://dev.mysql.com/downloads/mysql/) | 8.0 | Base de datos principal |
| [Xcode](https://developer.apple.com/xcode/) | 16.5+ | Builds iOS/macCatalyst (macOS) |

---

## Configuración Inicial

### 1. Clonar el repositorio

```bash
git clone https://github.com/Terrado26Aa/SistForm.git
cd SistForm
```

### 2. Instalar workloads de .NET MAUI

```bash
# Todas las plataformas
dotnet workload install maui

# Solo Android (Linux/CI)
dotnet workload install maui-android
```

Verificar instalación:
```bash
dotnet workload list
```

### 3. Configurar MySQL

Crear la base de datos:
```sql
CREATE DATABASE sistform CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

Aplicar migraciones:
```bash
dotnet ef database update --project AuthLogin/AuthLogin.csproj
```

O ejecutar el script SQL desde `Base de Datos/sistform.sql` (si prefieres importar directamente).

### 4. Configurar conexión

Editar `AuthLogin/appsettings.json` con tus credenciales MySQL:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=sistform;Uid=tu_usuario;Pwd=tu_contraseña;"
  }
}
```

### 5. Ejecutar el backend

```bash
cd AuthLogin
dotnet run
```

El backend arranca en `http://localhost:5174`.  
Swagger UI disponible en `http://localhost:5174/swagger`.

### 6. Ejecutar el frontend

```bash
cd Forms
dotnet build -f net10.0-android -t:Run -c Release
```

O desde Visual Studio: seleccionar plataforma (Android, Windows, etc.) y presionar F5.

---

## Estructura del Proyecto

```
SistForm/
├── Forms/                        # Frontend .NET MAUI
│   ├── MauiProgram.cs            # Punto de entrada, DI
│   ├── App.xaml / App.xaml.cs    # Configuración de la app
│   ├── AppShell.xaml             # Navegación (Shell)
│   ├── Config.cs                 # URL base de la API
│   ├── Models/                   # DTOs compartidos
│   ├── Services/
│   │   ├── ApiService.cs         # Cliente HTTP
│   │   └── LocalDatabaseHelper.cs # Almacenamiento offline
│   ├── ViewModels/               # MVVM ViewModels
│   ├── Views/                    # Páginas XAML
│   └── Platforms/                # Código específico por plataforma
│
├── AuthLogin/                    # Backend ASP.NET Core
│   ├── Program.cs                # Punto de entrada, middleware
│   ├── Controllers/              # API Controllers
│   ├── Services/                 # Lógica de negocio
│   ├── Models/                   # Entidades y DTOs
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Middleware/
│   │   └── GlobalExceptionMiddleware.cs
│   └── Migrations/               # Migraciones EF Core
│
├── AuthLogin.Tests/              # Tests unitarios
│   └── AuthServiceTests.cs
│
├── global.json                   # SDK version
├── Forms.sln                     # Solución
└── .github/workflows/            # CI/CD pipelines
```

---

## Comandos Útiles

### Backend

```bash
# Ejecutar en modo desarrollo
dotnet run --project AuthLogin

# Agregar migración
dotnet ef migrations add <Nombre> --project AuthLogin

# Aplicar migraciones
dotnet ef database update --project AuthLogin

# Ver migraciones pendientes
dotnet ef migrations list --project AuthLogin
```

### Frontend

```bash
# Compilar para Android
dotnet build Forms/Forms.csproj -f net10.0-android -c Release

# Compilar para iOS (macOS)
dotnet build Forms/Forms.csproj -f net10.0-ios -c Release

# Compilar para MacCatalyst (macOS)
dotnet build Forms/Forms.csproj -f net10.0-maccatalyst -c Release

# Compilar para Windows
dotnet build Forms/Forms.csproj -f net10.0-windows10.0.19041.0 -c Release
```

### Tests

```bash
dotnet test AuthLogin.Tests/AuthLogin.Tests.csproj -c Release
```

---

## Convenciones de Código

- **Lenguaje:** C# (archivos .cs), XAML (vistas)
- **Framework:** .NET MAUI con patrón MVVM
- **Backend:** ASP.NET Core con inyección de dependencias
- **Base de datos:** EF Core + MySQL (Code First)
- **Autenticación:** JWT Bearer Tokens
- **Tests:** xUnit + Moq (mocking) + EF Core InMemory

### Nombramiento
| Elemento | Convención | Ejemplo |
|----------|------------|---------|
| Clases | PascalCase | `ApiService`, `LoginViewModel` |
| Métodos | PascalCase | `GetAllFormsAsync()` |
| Variables | camelCase | `formId`, `userRole` |
| DTOs | Sufijo `Dto` | `LoginRequestDto` |
| Servicios | Sufijo `Service` | `AuthService` |
| Interfaces | Prefijo `I` | `IAuthService` |

---

## Roles de Usuario

| Rol | Acceso |
|-----|--------|
| **User** | Ver formularios asignados, responder encuestas, ver perfil propio |
| **Admin** | CRUD de formularios, asignar formularios, gestionar usuarios, dashboard, ver todas las respuestas |

---

## URLs por Plataforma

El frontend se conecta al backend usando la URL definida en `Forms/Config.cs`:

```csharp
public static string BaseApiUrl = DeviceInfo.Platform == DevicePlatform.Android 
    ? "http://10.0.2.2:5174"     // Android Emulator → localhost host
    : "http://127.0.0.1:5174";    // Otras plataformas
```

Para iOS simulator también puedes usar `http://127.0.0.1:5174`.  
Para dispositivos físicos, reemplazar con la IP local del servidor.
