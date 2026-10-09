# Genera pedidos contra el sistema para que los gráficos tengan datos durante la demo.
# Solo hace consultas (GET) y pedidos inválidos que la API rechaza: no modifica la base.
# Uso local:  powershell -ExecutionPolicy Bypass -File .\generar-trafico.ps1
# Uso Azure:  powershell -ExecutionPolicy Bypass -File .\generar-trafico.ps1 -Api "https://<dominio-api>" -Web "https://<dominio-web>"

param(
    [string]$Api = "http://localhost:5215",
    [string]$Web = "http://localhost:5012",
    [int]$Segundos = 120
)

# Pedidos "normales" (deberían responder 200)
$normales = @(
    "$Api/api/actividades",
    "$Api/api/suscripciones",
    "$Api/api/anuncios",
    "$Api/api/profesores",
    "$Web/"
)

function Pedir([string]$url, [string]$metodo = "GET", [string]$cuerpo = $null) {
    try {
        if ($cuerpo) {
            $r = Invoke-WebRequest -Uri $url -Method $metodo -Body $cuerpo -ContentType "application/json" -UseBasicParsing -TimeoutSec 60
        } else {
            $r = Invoke-WebRequest -Uri $url -Method $metodo -UseBasicParsing -TimeoutSec 60
        }
        return [int]$r.StatusCode
    } catch {
        if ($_.Exception.Response) { return [int]$_.Exception.Response.StatusCode }
        return 0   # sin respuesta (la app no está levantada)
    }
}

$fin = (Get-Date).AddSeconds($Segundos)
$contador = 0
Write-Host "Generando tráfico durante $Segundos segundos... (Ctrl+C para cortar)"

while ((Get-Date) -lt $fin) {
    $sorteo = Get-Random -Maximum 100

    if ($sorteo -lt 75) {
        $codigo = Pedir ($normales | Get-Random)                                 # 75 %: consultas normales
    } elseif ($sorteo -lt 90) {
        $codigo = Pedir "$Api/api/alumnos/$(Get-Random -Minimum 90000 -Maximum 99999)"  # 15 %: 404, no existe
    } else {
        $codigo = Pedir "$Api/api/actividades" "POST" "{}"                       # 10 %: 400, datos inválidos
    }

    $contador++
    Write-Host -NoNewline "$codigo "
    Start-Sleep -Milliseconds (Get-Random -Minimum 100 -Maximum 400)
}

Write-Host ""
Write-Host "Listo: $contador pedidos enviados."
