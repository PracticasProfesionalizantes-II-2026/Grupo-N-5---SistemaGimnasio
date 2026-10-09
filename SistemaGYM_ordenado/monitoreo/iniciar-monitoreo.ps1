# Inicia Prometheus (http://localhost:9090) y Grafana (http://localhost:3000), cada uno en su propia ventana.
# Antes hay que haber ejecutado .\instalar-herramientas.ps1 una vez.
# Uso:  powershell -ExecutionPolicy Bypass -File .\iniciar-monitoreo.ps1

$ErrorActionPreference = "Stop"

$herramientas = Join-Path $PSScriptRoot "herramientas"
$prometheus = Get-ChildItem $herramientas -Directory -Filter "prometheus-*" -ErrorAction SilentlyContinue | Select-Object -First 1
$grafana = Get-ChildItem $herramientas -Directory -Filter "grafana-*" -ErrorAction SilentlyContinue | Select-Object -First 1

if (-not $prometheus -or -not $grafana) {
    Write-Host "No se encontraron Prometheus o Grafana. Ejecutá primero .\instalar-herramientas.ps1"
    exit 1
}

$datos = Join-Path $PSScriptRoot "datos"
New-Item -ItemType Directory -Force (Join-Path $datos "prometheus"), (Join-Path $datos "grafana") | Out-Null

# Prometheus: lee prometheus.yml y guarda las mediciones en datos/prometheus
Start-Process -FilePath (Join-Path $prometheus.FullName "prometheus.exe") -WorkingDirectory $prometheus.FullName -ArgumentList @(
    "--config.file=`"$(Join-Path $PSScriptRoot 'prometheus.yml')`"",
    "--storage.tsdb.path=`"$(Join-Path $datos 'prometheus')`"",
    "--web.listen-address=127.0.0.1:9090"
)

# Grafana: se configura con variables de entorno (las hereda el proceso que se inicia)
$env:GF_SERVER_HTTP_ADDR = "127.0.0.1"
$env:GF_PATHS_DATA = Join-Path $datos "grafana"
$env:GF_PATHS_PROVISIONING = Join-Path $PSScriptRoot "grafana\provisioning"
$env:SISTEMAGYM_DASHBOARDS = Join-Path $PSScriptRoot "grafana\dashboards"
$env:GF_AUTH_ANONYMOUS_ENABLED = "true"       # entrar sin usuario (solo escucha en esta PC)
$env:GF_AUTH_ANONYMOUS_ORG_ROLE = "Admin"
$env:GF_DASHBOARDS_DEFAULT_HOME_DASHBOARD_PATH = Join-Path $PSScriptRoot "grafana\dashboards\sistemagym.json"

Start-Process -FilePath (Join-Path $grafana.FullName "bin\grafana.exe") -WorkingDirectory $grafana.FullName -ArgumentList @(
    "server", "--homepath", "`"$($grafana.FullName)`""
)

Write-Host "Prometheus: http://localhost:9090/targets"
Write-Host "Grafana:    http://localhost:3000  (tarda unos segundos en arrancar)"
