# Monitoreo del Sistema de Gestión de Gimnasio

Implementamos **3 métricas** del documento "Monitoreo de aplicaciones" con **Prometheus** y **Grafana**.

## ¿Para qué sirve cada herramienta?

| Herramienta | Qué hace | En nuestro proyecto |
|---|---|---|
| **prometheus-net** (NuGet) | Librería para .NET que mide los pedidos HTTP y los publica en la ruta `/metrics` en el formato de texto que entiende Prometheus. | La usan la API y la Web. |
| **Prometheus** | Base de datos de series de tiempo. Cada pocos segundos *lee* (hace "scrape" de) `/metrics` de cada aplicación y guarda los valores con fecha y hora. Se consulta con el lenguaje **PromQL**. | Lee la API (`:5215`) y la Web (`:5012`), y también las apps de Azure. |
| **Grafana** | Herramienta de visualización: se conecta a Prometheus y muestra las consultas PromQL como gráficos y tableros. | Tablero "Sistema de Gestión de Gimnasio · Métricas". |

```
 API (.NET) ──/metrics──┐
                        ├──►  Prometheus (guarda)  ──PromQL──►  Grafana (muestra)
 Web (.NET) ──/metrics──┘        :9090                             :3000
```

Prometheus trabaja con el modelo **pull**: la aplicación no envía nada, solo expone sus números y Prometheus los va a buscar.

## Las 3 métricas (Métricas de Aplicación y Confiabilidad)

Son las que el documento llama *cantidad de peticiones*, *tasa de error* y *tiempo de respuesta*. Para cada una definimos un **SLI** (el indicador) y un **SLO** (el objetivo).

### 1. Tráfico: cantidad de peticiones
Cuántos pedidos por segundo recibe cada aplicación. Sirve para ver la carga y los horarios pico (por ejemplo, cuando los alumnos se inscriben a las clases).
```promql
sum by (app) (rate(http_requests_received_total[1m]))
```

### 2. Tasa de error
Porcentaje de pedidos que terminan en error. Separamos **5xx** (falla del servidor, es culpa nuestra) de **4xx** (el pedido estaba mal: datos inválidos, algo que no existe).
**SLO:** menos del 1 % de errores 5xx.
```promql
100 * sum(rate(http_requests_received_total{code=~"5.."}[5m]))
    / sum(rate(http_requests_received_total[5m]))
```

### 3. Tiempo de respuesta
Cuánto tarda el sistema en responder. Mostramos el **p50** (la mitad de los pedidos tarda menos) y el **p95** (el 95 % tarda menos), y los endpoints de la API más lentos.
**SLO:** el 99 % de los pedidos responde en menos de 2 s. Sale de nuestro requerimiento no funcional de tiempo de respuesta (2 a 3 segundos).
```promql
histogram_quantile(0.95, sum by (le, app) (rate(http_request_duration_seconds_bucket[5m])))
```

**Extra (sin código adicional):** prometheus-net también publica disponibilidad (`up`), uso de CPU y memoria. Están en la última fila del tablero.

## Cómo se implementó en .NET

1. Paquete NuGet en los dos proyectos:
   ```
   dotnet add API/SistemaGYM_.csproj package prometheus-net.AspNetCore
   dotnet add SistemaGYM.Web/SistemaGYM.Web.csproj package prometheus-net.AspNetCore
   ```
2. Dos líneas en cada `Program.cs`:
   ```csharp
   using Prometheus;
   ...
   app.UseHttpMetrics();  // mide cada pedido: cantidad, código de respuesta y duración
   ...
   app.MapMetrics();      // publica las métricas en /metrics
   ```
   En la Web, `UseHttpMetrics()` va después de `UseRouting()` para saber qué controlador y acción atendió el pedido.


