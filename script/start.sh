#!/usr/bin/env bash
set -e

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"

start_backend() {
    echo "=== Iniciando backend API ==="
    cd "$ROOT_DIR/SistForm-API"
    dotnet run &
    BACKEND_PID=$!
    echo "Backend iniciado (PID: $BACKEND_PID)"
}

start_frontend() {
    echo "=== Iniciando frontend MAUI ==="
    cd "$ROOT_DIR/Forms"
    case "$1" in
        android)
            dotnet build -f net10.0-android -t:Run -c Release
            ;;
        ios)
            dotnet build -f net10.0-ios -t:Run -c Release
            ;;
        windows)
            dotnet build -f net10.0-windows10.0.19041.0 -t:Run -c Release
            ;;
        linux-gtk4)
            echo "=== Iniciando frontend Linux GTK4 ==="
            cd "$ROOT_DIR/Forms.Linux"
            dotnet run -c Release
            ;;
        maccatalyst)
            dotnet run -f net10.0-maccatalyst -c Release
            ;;
        *)
            echo "Uso: $0 {android|ios|windows|maccatalyst}"
            exit 1
            ;;
    esac
}

start_docker() {
    echo "=== Iniciando contenedores Docker ==="
    cd "$ROOT_DIR/script"
    docker compose up -d
    echo "Docker compose iniciado."
    echo "  MySQL:  localhost:3307"
    echo "  API:    http://localhost:5174"
}

stop_docker() {
    echo "=== Deteniendo contenedores Docker ==="
    cd "$ROOT_DIR/script"
    docker compose down
    echo "Contenedores detenidos."
}

case "${1:-}" in
    backend)
        start_backend
        wait $BACKEND_PID
        ;;
    android|ios|windows|maccatalyst|linux-gtk4)
        start_backend
        start_frontend "$1"
        wait
        ;;
    docker)
        start_docker
        ;;
    stop)
        stop_docker
        ;;
    *)
        echo "Uso: $0 {backend|android|ios|windows|maccatalyst|docker|stop}"
        echo ""
        echo "Ejemplos:"
        echo "  $0 backend          # Solo inicia la API"
        echo "  $0 android          # API + app Android"
        echo "  $0 ios              # API + app iOS (macOS)"
        echo "  $0 windows          # API + app Windows"
        echo "  $0 linux-gtk4       # API + app Linux GTK4"
        echo "  $0 maccatalyst      # API + app MacCatalyst"
        echo "  $0 docker           # Inicia MySQL + API en Docker"
        echo "  $0 stop             # Detiene Docker"
        exit 1
        ;;
esac
