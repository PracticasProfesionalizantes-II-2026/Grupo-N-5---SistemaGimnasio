using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Administrador")]
public class PagosController : Controller
{
    private readonly IPagoApiService _pagoService;
    private readonly IAlumnoApiService _alumnoService;
    private readonly IAlumnoSuscripcionApiService _alumnoSuscripcionService;

    public PagosController(IPagoApiService pagoService, IAlumnoApiService alumnoService, IAlumnoSuscripcionApiService alumnoSuscripcionService)
    {
        _pagoService = pagoService;
        _alumnoService = alumnoService;
        _alumnoSuscripcionService = alumnoSuscripcionService;
    }

    // GET /Pagos -> Reporte de pagos (con filtro simple por fecha)
    public async Task<IActionResult> Index(DateTime? fechaDesde, DateTime? fechaHasta)
    {
        var pagos = await _pagoService.ObtenerTodosAsync();

        if (fechaDesde.HasValue)
            pagos = pagos.Where(p => p.FechaPago.Date >= fechaDesde.Value.Date).ToList();
        if (fechaHasta.HasValue)
            pagos = pagos.Where(p => p.FechaPago.Date <= fechaHasta.Value.Date).ToList();

        ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
        ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
        ViewBag.TotalIngresado = pagos.Sum(p => p.Monto);

        return View(pagos);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Alumnos = await _alumnoService.ObtenerTodosAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PagoCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Error = "Revisá los datos ingresados antes de registrar el pago.";
            ViewBag.Alumnos = await _alumnoService.ObtenerTodosAsync();
            return View(dto);
        }

        var (ok, error) = await _pagoService.CrearAsync(dto);
        if (!ok)
        {
            ViewBag.Error = error;
            ViewBag.Alumnos = await _alumnoService.ObtenerTodosAsync();
            return View(dto);
        }

        TempData["Mensaje"] = "Pago registrado con éxito en el sistema";
        return RedirectToAction("Index");
    }

    // GET /Pagos/SuscripcionesDeAlumno/5 -> usado por JS para poblar el segundo combo
    [HttpGet]
    public async Task<JsonResult> SuscripcionesDeAlumno(int id)
    {
        var historial = await _alumnoSuscripcionService.ObtenerHistorialAsync(id);
        var hoy = DateTime.UtcNow.Date;
        var opciones = historial
            .Where(h => h.Activa && (!h.FechaFin.HasValue || h.FechaFin.Value.Date >= hoy))
            .Select(h => new { id = h.Id, label = $"{h.NombrePlan} - vigente hasta {h.FechaFin?.ToString("dd/MM/yyyy")} - ${h.Precio}" });
        return Json(opciones);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _pagoService.EliminarAsync(id);
        TempData["Mensaje"] = "El pago se ha eliminado con éxito del sistema";
        return RedirectToAction("Index");
    }
}
