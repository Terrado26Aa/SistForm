# API REST

Documentación de los endpoints del backend **AuthLogin** (ASP.NET Core).  
URL base: `http://localhost:5174`

---

## Autenticación

Todas las rutas de `FormsController` y algunas de `AuthController` requieren el header:

```
Authorization: Bearer <token>
```

El token se obtiene del endpoint `POST /api/auth/login`.

---

## AuthController — `api/auth`

### `POST /api/auth/login`
Autentica un usuario y devuelve un token JWT.

**Request:**
```json
{
  "userName": "jperez",
  "password": "MiPassword123"
}
```

**Response (200):**
```json
{
  "message": "Inicio de sesion exitoso.",
  "userId": 1,
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "role": "Admin"
}
```

**Response (401):** `Credenciales invalidas.`

---

### `POST /api/auth/register`
Registra un nuevo usuario con rol `User`.

**Request:**
```json
{
  "username": "jperez",
  "firstname": "Juan",
  "lastname": "Perez",
  "email": "juan@example.com",
  "password": "MiPassword123",
  "role": "User"
}
```

**Response (200):**
```json
{ "message": "Usuario registrado exitosamente." }
```

---

### `GET /api/auth/profile/{id}` 🔒
Obtiene el perfil del usuario autenticado (solo puede ver su propio perfil).

**Response (200):**
```json
{
  "firstName": "Juan",
  "lastName": "Perez",
  "email": "juan@example.com",
  "role": "Admin",
  "newPassword": null
}
```

---

### `PUT /api/auth/profile/{id}` 🔒
Actualiza el perfil del usuario autenticado.

**Request:**
```json
{
  "firstName": "Juan Carlos",
  "lastName": "Perez",
  "email": "juanc@example.com",
  "newPassword": "NuevaClave456"
}
```

**Response (200):**
```json
{ "message": "Perfil actualizado exitosamente." }
```

---

### `POST /api/auth/register-admin` 🔒 (Admin)
Registra un nuevo usuario con rol especificado.

Mismos campos que `register`.  
**Response (200):** `{ "message": "Usuario registrado exitosamente." }`

---

### `GET /api/auth/users` 🔒 (Admin)
Lista todos los usuarios del sistema.

**Response (200):**
```json
[
  {
    "id": 1,
    "userName": "jperez",
    "firstName": "Juan",
    "lastName": "Perez",
    "email": "juan@example.com",
    "role": "Admin"
  }
]
```

---

### `PUT /api/auth/users/{id}/role` 🔒 (Admin)
Cambia el rol de un usuario.

**Request:** `"Admin"` o `"User"` (string plano)

**Response (200):**
```json
{ "message": "Rol actualizado exitosamente." }
```

---

### `DELETE /api/auth/users/{id}` 🔒 (Admin)
Elimina un usuario del sistema.

**Response (200):**
```json
{ "message": "Usuario eliminado exitosamente." }
```

---

## FormsController — `api/forms`

Todos los endpoints requieren autenticación `[Authorize]`.
Los endpoints marcados como **(Admin)** requieren rol `Admin`.

### `POST /api/forms` 🔒
Crea un nuevo formulario.

**Request:**
```json
{
  "idUser": 1,
  "title": "Encuesta de Biodiversidad",
  "description": "Registro de especies observadas",
  "elements": [
    {
      "id": 0,
      "title": "Nombre de la especie",
      "type": "text",
      "options": null,
      "maxSelections": null
    },
    {
      "id": 0,
      "title": "Tipo de bosque",
      "type": "select",
      "options": ["Seco", "Humedo", "Nublado"],
      "maxSelections": 1
    },
    {
      "id": 0,
      "title": "Especies avistadas",
      "type": "checkbox",
      "options": ["Panama", "Mono aullador", "Tucan"],
      "maxSelections": 3
    }
  ]
}
```

**Response (200):**
```json
{ "message": "Formulario creado exitosamente.", "formId": 1 }
```

**Tipos de elemento soportados:** `text`, `select`, `checkbox`, `number`, `date`, `textarea`, `email`, `phone`, `time`.

---

### `GET /api/forms/all` 🔒
Obtiene la lista de formularios.
- **Admin:** todos los formularios del sistema.
- **User:** solo los formularios asignados.

**Response (200):**
```json
[
  {
    "idForm": 1,
    "title": "Encuesta de Biodiversidad",
    "description": "Registro de especies observadas"
  }
]
```

---

### `GET /api/forms/{id}` 🔒
Obtiene un formulario completo con sus elementos.

**Response (200):**
```json
{
  "idForm": 1,
  "idUser": 1,
  "title": "Encuesta de Biodiversidad",
  "description": "Registro de especies observadas",
  "creationDate": "2026-04-25T00:00:00",
  "lastModifiedDate": "2026-04-25T00:00:00",
  "elements": [
    {
      "id": 1,
      "type": "text",
      "title": "Nombre de la especie",
      "options": null,
      "maxSelections": null
    }
  ]
}
```

---

### `PUT /api/forms/{id}` 🔒
Actualiza un formulario existente.

**Request:** Misma estructura que `POST /api/forms`.  
**Response (200):** `{ "message": "Formulario actualizado exitosamente." }`

---

### `DELETE /api/forms/{id}` 🔒
Elimina un formulario.

**Response (200):** `{ "message": "Formulario eliminado exitosamente." }`

---

### `POST /api/forms/submit` 🔒
Envía una respuesta a un formulario. Incluye soporte para geolocalización (coordenadas y ruta GeoJSON).

**Request:**
```json
{
  "formId": 1,
  "userId": 2,
  "responses": [
    {
      "formElementId": 1,
      "question": "Nombre de la especie",
      "answer": "Panama"
    },
    {
      "formElementId": 3,
      "question": "Especies avistadas",
      "answer": "Mono aullador, Tucan"
    }
  ],
  "formTitle": "Encuesta de Biodiversidad",
  "saveAt": "2026-04-25T14:30:00",
  "latitudeA": 8.9731,
  "longitudeA": -79.5206,
  "latitudeB": 8.9750,
  "longitudeB": -79.5220,
  "trackPoints": [
    { "lat": 8.9731, "lon": -79.5206 },
    { "lat": 8.9740, "lon": -79.5210 },
    { "lat": 8.9750, "lon": -79.5220 }
  ]
}
```

**Response (200):**
```json
{ "message": "Respuesta guardada exitosamente." }
```

---

### `GET /api/forms/{id}/results` 🔒
Obtiene todas las respuestas de un formulario.

**Response (200):**
```json
[
  {
    "id": 1,
    "formId": 1,
    "userId": 2,
    "date": "2026-04-25T14:30:00",
    "latitudeA": 8.9731,
    "longitudeA": -79.5206,
    "latitudeB": 8.9750,
    "longitudeB": -79.5220,
    "routePath": "{\"type\":\"LineString\",\"coordinates\":[[-79.5206,8.9731],[-79.5210,8.9740],[-79.5220,8.9750]]}",
    "details": [
      {
        "id": 1,
        "responseId": 1,
        "questionTitle": "Nombre de la especie",
        "answer": "Panama"
      }
    ]
  }
]
```

---

### `POST /api/forms/assign` 🔒 (Admin)
Asigna un formulario a un usuario.

**Request:**
```json
{
  "formId": 1,
  "userId": 2
}
```

**Response (200):**
```json
{ "message": "Formulario asignado exitosamente." }
```

---

### `GET /api/forms/users` 🔒 (Admin)
Lista usuarios para asignación de formularios.

**Response (200):**
```json
[
  {
    "id": 2,
    "userName": "jperez",
    "firstName": "Juan",
    "lastName": "Perez",
    "role": "User"
  }
]
```

---

### `GET /api/forms/dashboard-stats` 🔒 (Admin)
Estadísticas del dashboard.

**Response (200):**
```json
{
  "totalForms": 15,
  "totalUsers": 25,
  "totalResponses": 120,
  "recentForms": [
    {
      "idForm": 15,
      "title": "Encuesta de Suelos",
      "description": "Analisis de composicion del suelo",
      "creationDate": "2026-04-25T00:00:00"
    }
  ]
}
```

---

## Códigos de Estado

| Código | Significado |
|--------|-------------|
| 200 | Éxito |
| 400 | Bad Request — datos inválidos |
| 401 | Unauthorized — token ausente o inválido |
| 403 | Forbidden — sin permisos suficientes |
| 404 | Not Found — recurso no existe |
| 500 | Internal Server Error |

## Esquema de Autenticación (Swagger)

El header `Authorization: Bearer <token>` se puede configurar en Swagger UI haciendo clic en **Authorize** e ingresando el token.
