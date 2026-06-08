# v4.2 — .NET 10 y CI/CD

**Fase:** Migracion y DevOps (v4.2)

Migración de .NET 8 a .NET 10 y corrección del pipeline de CI/CD multiplataforma.

---

## Migración a .NET 10

### global.json
```json
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestFeature"
  }
}
```

### Frontend (Forms.csproj)
- **TFMs:** `net10.0-android`, `net10.0-ios`, `net10.0-maccatalyst`, `net10.0-windows10.0.19041.0`
- `Microsoft.Maui.Controls` → 10.0.60
- `CommunityToolkit.Maui` → 14.1.1
- `CommunityToolkit.Mvvm` → 8.4.2
- `Microsoft.Extensions.Logging.Debug` → 10.0.5
- `Microsoft.Maui.Controls.Maps` → 10.0.60

### Backend (AuthLogin.csproj)
- **TFM:** `net10.0`
- `Microsoft.AspNetCore.Authentication.JwtBearer` → 10.0.6
- `Microsoft.EntityFrameworkCore` → 10.0.6
- `Microsoft.EntityFrameworkCore.Tools` → 10.0.7
- `Microting.EntityFrameworkCore.MySql` → 10.0.5 (fork de Pomelo)
- `Serilog.AspNetCore` → 10.0.0
- `Swashbuckle.AspNetCore` → 10.1.7

### Tests (AuthLogin.Tests.csproj)
- **TFM:** `net10.0`
- Paquetes actualizados a versiones compatibles con .NET 10

---

## Fixes de CI/CD

### Problema
El SDK de iOS para .NET 10 requiere Xcode 26.5, pero `macos-latest` tiene Xcode 16.4.

### Solución
- Cambiar runner de `macos-latest` a `macos-26` para builds de iOS y macCatalyst
- Agregar paso `sudo xcode-select -s /Applications/Xcode_26.5.app` antes del build
- Agregar `continue-on-error: true` para evitar fallos en cascada

### Release
- Filtrar artefactos a solo ejecutables: `*.apk`, `*.app`, `*.exe`
- Agregar `permissions: contents: write`
- Actualizar rutas de artefactos a `net10.0-*`

---

## Archivos Modificados

| Archivo | Cambio |
|---------|--------|
| `global.json` | SDK 10.0.100 |
| `Forms/Forms.csproj` | TFMs y paquetes a .NET 10 |
| `AuthLogin/AuthLogin.csproj` | TFM y paquetes a .NET 10, Microting.MySql |
| `AuthLogin/Program.cs` | Swashbuckle v10 / OpenApi v2 |
| `AuthLogin.Tests/AuthLogin.Tests.csproj` | TFM y paquetes a .NET 10 |
| `.github/workflows/release.yml` | Runner macos-26, Xcode select, net10.0 rutas |
| `.github/workflows/ci.yml` | DOTNET_VERSION 10.0.x |
| `README.md` | Requisito .NET 10 |
| `wiki/README.md` | Requisito .NET 10 |
