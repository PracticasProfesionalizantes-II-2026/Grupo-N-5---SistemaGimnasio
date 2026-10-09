# Descarga Prometheus y Grafana (versiones para Windows, sin instalador) en monitoreo/herramientas.
# Se hace una sola vez por PC. La carpeta herramientas/ no se sube a GitHub.
# Uso:  powershell -ExecutionPolicy Bypass -File .\instalar-herramientas.ps1

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"   # sin esto Invoke-WebRequest descarga muy lento

$versionPrometheus = "3.5.0"
$versionGrafana = "12.1.1"

$carpeta = Join-Path $PSScriptRoot "herramientas"
New-Item -ItemType Directory -Force $carpeta | Out-Null

$descargas = @(
    @{ Nombre = "Prometheus"; Carpeta = "prometheus-$versionPrometheus.windows-amd64";
       Url = "https://github.com/prometheus/prometheus/releases/download/v$versionPrometheus/prometheus-$versionPrometheus.windows-amd64.zip" },
    @{ Nombre = "Grafana"; Carpeta = "grafana-$versionGrafana";
       Url = "https://dl.grafana.com/oss/release/grafana-$versionGrafana.windows-amd64.zip" }
)

foreach ($d in $descargas) {
    if (Test-Path (Join-Path $carpeta $d.Carpeta)) {
        Write-Host "$($d.Nombre) ya está descargado."
        continue
    }
    $zip = Join-Path $carpeta "$($d.Nombre).zip"
    Write-Host "Descargando $($d.Nombre) (puede tardar unos minutos)..."
    Invoke-WebRequest -Uri $d.Url -OutFile $zip
    Write-Host "Descomprimiendo $($d.Nombre)..."
    Expand-Archive -Path $zip -DestinationPath $carpeta -Force
    Remove-Item $zip
}

Write-Host "Listo. Ahora ejecutá .\iniciar-monitoreo.ps1"
