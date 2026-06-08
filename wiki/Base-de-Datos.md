# Base de Datos

SistForm utiliza **MySQL 8.0** como base de datos principal y **SQLite** local para soporte offline.

---

## MySQL (Producción)

### Esquema Relacional

```
┌───────────────────┐
│      users        │
├───────────────────┤
│ Id (PK)           │──┐
│ UserName (UQ)     │  │
│ FirstName         │  │
│ LastName          │  │
│ PasswordHash      │  │
│ Email             │  │
│ Role (User/Admin) │  │
└───────────────────┘  │
                       │
┌───────────────────┐  │
│  createforms       │  │
├───────────────────┤  │
│ IdForm (PK)       │  │
│ IdUser            │──┘
│ Title             │
│ Description       │
│ CreationDate      │
│ LastModifiedDate  │
└────────┬──────────┘  │
         │             │
┌────────▼──────────┐  │
│  form_elements     │  │
├───────────────────┤  │
│ Id (PK)           │  │
│ Type              │  │
│ Title             │  │
│ Options (JSON)    │  │
│ MaxSelections     │  │
│ IdForm (FK) ──────┘  │
└───────────────────┘  │
                       │
┌───────────────────┐  │
│ form_responses     │  │
├───────────────────┤  │
│ Id (PK)           │  │
│ FormId (FK) ──────┘  │
│ UserId (FK) ─────────┘
│ Date               │
│ LatitudeA          │
│ LongitudeA         │
│ LatitudeB          │
│ LongitudeB         │
│ RoutePath (GeoJSON)│
└────────┬──────────┘
         │
┌────────▼──────────┐
│form_response_details│
├───────────────────┤
│ Id (PK)           │
│ ResponseId (FK) ──┘
│ QuestionTitle     │
│ Answer            │
└───────────────────┘

┌───────────────────┐
│ form_assignments   │
├───────────────────┤
│ Id (PK)           │
│ FormId (FK) ──────┐
│ UserId (FK) ──────┤
│ AssignedDate      │
└───────────────────┘
```

---

### Tablas

#### `users`
| Columna | Tipo | Descripción |
|---------|------|-------------|
| `Id` | INT (PK, AUTO_INCREMENT) | Identificador único |
| `UserName` | VARCHAR (UQ) | Nombre de usuario |
| `FirstName` | VARCHAR | Nombre real |
| `LastName` | VARCHAR | Apellido |
| `PasswordHash` | VARCHAR | Hash BCrypt de la contraseña |
| `Email` | VARCHAR | Correo electrónico |
| `Role` | VARCHAR (Default: "User") | Rol: `User` o `Admin` |

#### `createforms`
| Columna | Tipo | Descripción |
|---------|------|-------------|
| `IdForm` | INT (PK, AUTO_INCREMENT) | Identificador único |
| `IdUser` | INT (FK → users.Id) | Creador del formulario |
| `Title` | VARCHAR | Título del formulario |
| `Description` | TEXT | Descripción opcional |
| `CreationDate` | DATETIME | Fecha de creación |
| `LastModifiedDate` | DATETIME | Última modificación |

#### `form_elements`
| Columna | Tipo | Descripción |
|---------|------|-------------|
| `Id` | INT (PK, AUTO_INCREMENT) | Identificador único |
| `Type` | VARCHAR | Tipo: `text`, `select`, `checkbox`, `number`, `date`, `textarea`, `email`, `phone`, `time` |
| `Title` | VARCHAR | Texto de la pregunta |
| `Options` | JSON | Opciones separadas por coma (para select/checkbox) |
| `MaxSelections` | INT (Default: 0) | Máximo de selecciones (0 = sin límite) |
| `IdForm` | INT (FK → createforms.IdForm) | Formulario al que pertenece |

#### `form_responses`
| Columna | Tipo | Descripción |
|---------|------|-------------|
| `Id` | INT (PK, AUTO_INCREMENT) | Identificador único |
| `FormId` | INT (FK → createforms.IdForm) | Formulario respondido |
| `UserId` | INT (FK → users.Id) | Usuario que respondió |
| `Date` | DATETIME | Fecha de respuesta |
| `LatitudeA` | DOUBLE | Coordenada A latitud (inicio) |
| `LongitudeA` | DOUBLE | Coordenada A longitud (inicio) |
| `LatitudeB` | DOUBLE | Coordenada B latitud (fin) |
| `LongitudeB` | DOUBLE | Coordenada B longitud (fin) |
| `RoutePath` | TEXT (JSON) | Ruta recorrida en formato GeoJSON LineString |

#### `form_response_details`
| Columna | Tipo | Descripción |
|---------|------|-------------|
| `Id` | INT (PK, AUTO_INCREMENT) | Identificador único |
| `ResponseId` | INT (FK → form_responses.Id) | Respuesta padre |
| `QuestionTitle` | VARCHAR | Texto de la pregunta (guardada al momento de responder) |
| `Answer` | TEXT | Respuesta del usuario |

#### `form_assignments`
| Columna | Tipo | Descripción |
|---------|------|-------------|
| `Id` | INT (PK, AUTO_INCREMENT) | Identificador único |
| `FormId` | INT (FK → createforms.IdForm) | Formulario asignado |
| `UserId` | INT (FK → users.Id) | Usuario asignado |
| `AssignedDate` | DATETIME | Fecha de asignación |

---

### Script de Creación

```sql
CREATE TABLE users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserName VARCHAR(255) NOT NULL UNIQUE,
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    Role VARCHAR(50) DEFAULT 'User'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE createforms (
    IdForm INT AUTO_INCREMENT PRIMARY KEY,
    IdUser INT NOT NULL,
    Title VARCHAR(500) NOT NULL,
    Description TEXT,
    CreationDate DATETIME NOT NULL,
    LastModifiedDate DATETIME NOT NULL,
    FOREIGN KEY (IdUser) REFERENCES users(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE form_elements (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Type VARCHAR(100) NOT NULL,
    Title VARCHAR(500) NOT NULL,
    Options TEXT,
    MaxSelections INT DEFAULT 0,
    IdForm INT NOT NULL,
    FOREIGN KEY (IdForm) REFERENCES createforms(IdForm) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE form_responses (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FormId INT NOT NULL,
    UserId INT NOT NULL,
    Date DATETIME NOT NULL,
    LatitudeA DOUBLE,
    LongitudeA DOUBLE,
    LatitudeB DOUBLE,
    LongitudeB DOUBLE,
    RoutePath TEXT,
    FOREIGN KEY (FormId) REFERENCES createforms(IdForm),
    FOREIGN KEY (UserId) REFERENCES users(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE form_response_details (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ResponseId INT NOT NULL,
    QuestionTitle VARCHAR(500) NOT NULL,
    Answer TEXT,
    FOREIGN KEY (ResponseId) REFERENCES form_responses(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE form_assignments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FormId INT NOT NULL,
    UserId INT NOT NULL,
    AssignedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (FormId) REFERENCES createforms(IdForm),
    FOREIGN KEY (UserId) REFERENCES users(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
```

---

## SQLite (Offline Local)

El almacenamiento offline usa archivos JSON en lugar de SQLite propiamente. `LocalDatabaseHelper` gestiona:

| Archivo | Propósito |
|---------|-----------|
| `pending_request.json` | Respuestas pendientes de sincronizar |
| `forms_data_{userId}.json` | Formularios descargados para uso offline |

### Flujo Offline

```
Usuario responde sin conexión
        │
        ▼
SaveResponseLocallyAsync()
  → Guarda en pending_request.json
        │
        ▼
Cuando hay conexión
  → SyncSurveysPage lee pending_request.json
  → Envía cada respuesta a POST /api/forms/submit
  → Si éxito: DeletePendingResponseAsync()
```

### Métodos de LocalDatabaseHelper

| Método | Descripción |
|--------|-------------|
| `SaveResponseLocallyAsync(dto)` | Guarda respuesta pendiente |
| `GetPendingResponsesAsync()` | Lista respuestas pendientes |
| `DeletePendingResponseAsync(localId)` | Elimina respuesta pendiente (tras sincronizar) |
| `ClearPendingResponses()` | Limpia todas las respuestas pendientes |
| `SaveFormLocallyAsync(dto)` | Descarga formulario para offline |
| `GetDownloadedFormByIdAsync(id)` | Obtiene formulario offline por ID |
| `DeleteDownloadedFormAsync(id)` | Elimina formulario offline |
| `GetAllDownloadedFormAsync()` | Lista formularios descargados |

---

## Entity Framework Core Migrations

Las migraciones se generan con:

```bash
dotnet ef migrations add <NombreMigracion> \
    --project AuthLogin/AuthLogin.csproj
```

La migración existente `20260425012611_AddCoordinates` agregó las columnas de coordenadas y ruta a `form_responses`.

### Cadena de Conexión

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=sistform;Uid=cesart;Pwd=12345;"
  }
}
```
