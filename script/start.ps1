param(
    [Parameter(Position=0)]
    [ValidateSet('backend','android','ios','windows','maccatalyst','linux-gtk4','docker','stop')]
    [string]$Command = ''
)

$ROOT_DIR = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)

function Start-Backend {
    Write-Host "=== Iniciando SistForm-API ==="
    $apiDir = Join-Path $ROOT_DIR "SistForm-API"
    Set-Location $apiDir
    Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "run" -PassThru
}

function Start-Frontend($platform) {
    Write-Host "=== Iniciando frontend MAUI ($platform) ==="
    $formsDir = Join-Path $ROOT_DIR "Forms"

    switch ($platform) {
        "android"   { $tfm = "net10.0-android" }
        "ios"       { $tfm = "net10.0-ios" }
        "windows"   { $tfm = "net10.0-windows10.0.19041.0" }
        "maccatalyst" { $tfm = "net10.0-maccatalyst" }
    }

    Set-Location $formsDir
    dotnet build -f $tfm -t:Run -c Release
}

function Start-LinuxGtk4 {
    Write-Host "=== Iniciando frontend Linux GTK4 ==="
    $linuxDir = Join-Path $ROOT_DIR "Forms.Linux"
    Set-Location $linuxDir
    dotnet run -c Release
}

function Start-Docker {
    Write-Host "=== Iniciando contenedores Docker ==="
    $scriptDir = Join-Path $ROOT_DIR "script"
    Set-Location $scriptDir
    docker compose up -d
    Write-Host "Docker compose iniciado."
    Write-Host "  MySQL:  localhost:3307"
    Write-Host "  API:    http://localhost:5174"
}

function Stop-Docker {
    Write-Host "=== Deteniendo contenedores Docker ==="
    $scriptDir = Join-Path $ROOT_DIR "script"
    Set-Location $scriptDir
    docker compose down
    Write-Host "Contenedores detenidos."
}

switch ($Command) {
    "backend" {
        $process = Start-Backend
        Write-Host "Backend iniciado (PID: $($process.Id))"
        Wait-Process -Id $process.Id
    }
    "android" { Start-Backend; Start-Frontend "android" }
    "ios" { Start-Backend; Start-Frontend "ios" }
    "windows" { Start-Backend; Start-Frontend "windows" }
    "maccatalyst" { Start-Backend; Start-Frontend "maccatalyst" }
    "linux-gtk4" { Start-Backend; Start-LinuxGtk4 }
    "docker" { Start-Docker }
    "stop" { Stop-Docker }
    default {
        Write-Host "Uso: $PSCommandPath {backend|android|ios|windows|maccatalyst|linux-gtk4|docker|stop}"
        Write-Host ""
        Write-Host "Ejemplos:"
        Write-Host "  $PSCommandPath backend          # Solo inicia la API"
        Write-Host "  $PSCommandPath android          # API + app Android"
        Write-Host "  $PSCommandPath windows          # API + app Windows"
        Write-Host "  $PSCommandPath linux-gtk4       # API + app Linux GTK4"
        Write-Host "  $PSCommandPath docker           # Inicia MySQL + API en Docker"
        Write-Host "  $PSCommandPath stop             # Detiene Docker"
    }
}
