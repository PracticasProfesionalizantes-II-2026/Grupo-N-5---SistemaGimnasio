using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Models;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

// Todos los reportes se calculan acá, en la Web, con datos que la API ya devuelve.
// No hace falta ningún endpoint nuevo ni cambios en la base de datos.
[SessionAuthorize("Administrador")]
public class ReportesController : Controller
{
    private readonly IAlumnoApiService _alumnoService;
    private readonly IAlumnoSuscripcionApiService _alumnoSuscripcionService;
    private readonly IActividadApiService _actividadService;
    private readonly IPagoApiService _pagoService;

    public ReportesController(
        IAlumnoApiService alumnoService,
        IAlumnoSuscripcionApiService alumnoSuscripcionService,
        IActividadApiService actividadService,
        IPagoApiService pagoService)
    {
        _alumnoService = alumnoService;
        _alumnoSuscripcionService = alumnoSuscripcionService;
        _actividadService = actividadService;
        _pagoService = pagoService;
    }

    // GET /Reportes -> menú de reportes
    public IActionResult Index() => View();

    // GET /Reportes/Suscripciones?soloActivas=true|false
    public async Task<IActionResult> Suscripciones(bool soloActivas = true)
    {
        var filas = new List<(AlumnoDto Alumno, AlumnoSuscripcionDto Suscripcion)>();

        foreach (var (alumno, historial) in await HistorialDeTodosAsync())
            foreach (var s in historial.Where(s => s.Activa == soloActivas))
                filas.Add((alumno, s));

        ViewBag.SoloActivas = soloActivas;
        return View(filas);
    }

    // GET /Reportes/PorVencer -> suscripciones activas que vencen en los próximos 7 días
    public async Task<IActionResult> PorVencer()
    {
        var hoy = DateTime.Today;
        var limite = hoy.AddDays(7);
        var filas = new List<(AlumnoDto Alumno, AlumnoSuscripcionDto Suscripcion)>();

        foreach (var (alumno, historial) in await HistorialDeTodosAsync())
            foreach (var s in historial.Where(s => s.Activa && s.FechaFin.HasValue
                                                   && s.FechaFin.Value.Date >= hoy && s.FechaFin.Value.Date <= limite))
                filas.Add((alumno, s));

        return View(filas.OrderBy(f => f.Suscripcion.FechaFin).ToList());
    }

    // GET /Reportes/SuscriptosPorMes -> cuántas suscripciones empezaron en cada uno de los últimos 12 meses
    public async Task<IActionResult> SuscriptosPorMes()
    {
        var inicios = new List<DateTime>();
        foreach (var (_, historial) in await HistorialDeTodosAsync())
            inicios.AddRange(historial.Select(s => s.FechaInicio));

        var datos = UltimosMeses(12)
            .Select(mes => new DatoMensual(mes, inicios.Count(f => f.Year == mes.Year && f.Month == mes.Month)))
            .ToList();

        return View(datos);
    }

    // GET /Reportes/ClientesPorPlan -> cuántos clientes tiene cada plan (según su suscripción activa)
    public async Task<IActionResult> ClientesPorPlan()
    {
        var alumnos = await _alumnoService.ObtenerTodosAsync();

        var datos = alumnos
            .GroupBy(a => a.Suscripcion)
            .Select(g => new DatoCategoria(g.Key, g.Count()))
            .OrderByDescending(d => d.Valor)
            .ToList();

        ViewBag.TotalClientes = alumnos.Count;
        return View(datos);
    }

    // GET /Reportes/Ingresos -> total cobrado por mes (últimos 12 meses) y por método de pago
    public async Task<IActionResult> Ingresos()
    {
        var pagos = await _pagoService.ObtenerTodosAsync();

        var porMes = UltimosMeses(12)
            .Select(mes => new DatoMensual(mes, pagos
                .Where(p => p.FechaPago.Year == mes.Year && p.FechaPago.Month == mes.Month)
                .Sum(p => p.Monto)))
            .ToList();

        var porMetodo = pagos
            .GroupBy(p => p.MetodoPago)
            .Select(g => new DatoCategoria(NombreMetodo(g.Key), g.Sum(p => p.Monto)))
            .OrderByDescending(d => d.Valor)
            .ToList();

        ViewBag.PorMetodo = porMetodo;
        ViewBag.Total = pagos.Sum(p => p.Monto);
        return View(porMes);
    }

    // GET /Reportes/Ocupacion -> inscriptos sobre el cupo de cada actividad, de la más llena a la más vacía
    public async Task<IActionResult> Ocupacion()
    {
        var actividades = await _actividadService.ObtenerTodasAsync();
        return View(actividades
            .OrderByDescending(a => a.Cupo == 0 ? 0 : (double)a.Inscriptos / a.Cupo)
            .ToList());
    }

    // ---------- Ayudas ----------

    // Trae el historial de suscripciones de cada cliente (lo usan varios reportes)
    private async Task<List<(AlumnoDto Alumno, List<AlumnoSuscripcionDto> Historial)>> HistorialDeTodosAsync()
    {
        var resultado = new List<(AlumnoDto Alumno, List<AlumnoSuscripcionDto> Historial)>();
        foreach (var alumno in await _alumnoService.ObtenerTodosAsync())
            resultado.Add((alumno, await _alumnoSuscripcionService.ObtenerHistorialAsync(alumno.Id)));
        return resultado;
    }

    // Primer día de cada uno de los últimos "cantidad" meses, del más viejo al actual
    private static List<DateTime> UltimosMeses(int cantidad)
    {
        var mesActual = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        return Enumerable.Range(0, cantidad)
            .Select(i => mesActual.AddMonths(i - cantidad + 1))
            .ToList();
    }

    private static string NombreMetodo(MetodoPago metodo) => metodo switch
    {
        MetodoPago.TarjetaCredito => "Tarjeta de crédito",
        MetodoPago.TarjetaDebito => "Tarjeta de débito",
        MetodoPago.Efectivo => "Efectivo",
        MetodoPago.TransferenciaBancaria => "Transferencia",
        _ => metodo.ToString()
    };
}
