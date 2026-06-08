# Pipeline CI/CD

SistForm utiliza **GitHub Actions** para integración continua y despliegue continuo.

---

## Workflows

### CI (`ci.yml`) — Integración Continua

Se ejecuta en cada `push` o `pull_request` a la rama `main`.

| Job | Runner | Pasos |
|-----|--------|-------|
| `build-and-test` | `ubuntu-latest` | Restore → Build backend → Build tests → Run tests |

```yaml
name: CI
on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]
jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 10.0.x
      - run: dotnet restore AuthLogin/AuthLogin.csproj
      - run: dotnet restore AuthLogin.Tests/AuthLogin.Tests.csproj
      - run: dotnet build AuthLogin/AuthLogin.csproj --no-restore -c Release
      - run: dotnet build AuthLogin.Tests/AuthLogin.Tests.csproj --no-restore -c Release
      - run: dotnet test AuthLogin.Tests/AuthLogin.Tests.csproj -c Release --no-build
```

---

### Release (`release.yml`) — Despliegue Multiplataforma

Se ejecuta al pushear un tag `v*` o manualmente desde `workflow_dispatch`.

**Jobs:**

```
test → build (matrix: 7 builds) → release
```

#### `test` — Pruebas unitarias
- **Runner:** `ubuntu-latest`
- **Comando:** `dotnet test AuthLogin.Tests -c Release`

#### `build` — Matrix multiplataforma

Usa una estrategia **matrix** con 7 configuraciones distribuidas en 2 categorías:

##### Frontend (MAUI) — 4 builds

| Plataforma | TFM | Runner | Extra | Artefacto |
|------------|-----|--------|-------|-----------|
| Android | `net10.0-android` | `ubuntu-latest` | Java 17, workload `maui-android` | `*.apk` |
| iOS | `net10.0-ios` | `macos-26` | Xcode 26.5, se empaqueta en `.zip` | `*.zip` |
| macOS (MacCatalyst) | `net10.0-maccatalyst` | `macos-26` | Xcode 26.5, se empaqueta en `.zip` | `*.zip` |
| Windows | `net10.0-windows10.0.19041.0` | `windows-latest` | workload `maui` | `*.exe` |

##### Backend (API) — 3 builds

| Plataforma | Runtime | Runner | Artefacto |
|------------|---------|--------|-----------|
| Linux x64 | `linux-x64` | `ubuntu-latest` | `backend-linux/` (ejecutable + DLLs) |
| Windows x64 | `win-x64` | `windows-latest` | `backend-win/*.exe` |
| macOS x64 | `osx-x64` | `macos-26` | `backend-macos/` (ejecutable + DLLs) |

El backend se publica con `dotnet publish --self-contained` para no requerir runtime instalado.

#### `release` — Publicar Release
- Descarga los 7 artefactos.
- Crea un Release en GitHub con:

| Origen | Archivos |
|--------|----------|
| Android | `*.apk` |
| iOS | `*.zip` |
| MacCatalyst | `*.zip` |
| Windows (MAUI) | `*.exe` |
| Backend Linux | `backend-linux/**` |
| Backend Windows | `backend-win/*.exe` |
| Backend macOS | `backend-macos/**` |

---

## Cómo Disparar un Release

```bash
git tag v<version>
git push origin v<version>
```

El workflow ejecuta tests, compila los 7 artefactos en paralelo y crea un Release en GitHub con todos los binarios listos para descargar.
