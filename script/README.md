# Scripts y Docker

Scripts de inicio y contenedores para ejecutar SistForm.

## Scripts de inicio

### `start.sh` (Linux/macOS)
```bash
# Solo inicia la API
./script/start.sh backend

# API + app Android
./script/start.sh android

# API + app Windows
./script/start.sh windows

# API + app Linux GTK4
./script/start.sh linux-gtk4

# Inicia MySQL + API en Docker
./script/start.sh docker

# Detiene Docker
./script/start.sh stop
```

### `start.ps1` (Windows PowerShell)
```powershell
.\script\start.ps1 backend
.\script\start.ps1 android
.\script\start.ps1 windows
.\script\start.ps1 docker
.\script\start.ps1 stop
```

### `start.bat` (Windows cmd)
```cmd
script\start.bat backend
script\start.bat windows
script\start.bat docker
```

## Docker

### `docker-compose.yml`
Levanta dos servicios:
- **mysql** — MySQL 8.0 en puerto `3307`, inicializado con `Base de Datos/sistform.sql`
- **api** — Compila y ejecuta SistForm-API, expuesta en `http://localhost:5174`

### `Dockerfile`
Build multi-etapa para SistForm-API:
1. `build` — Restaura y publica con .NET SDK 10.0
2. `runtime` — Imagen aspnet:10.0 minima que ejecuta la DLL

```bash
docker compose -f script/docker-compose.yml up -d
```
