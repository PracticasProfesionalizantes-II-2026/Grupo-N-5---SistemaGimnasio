using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Administrador")]
public class SuscripcionesController : Controller
{
    private readonly ISuscripcionApiService _suscripcionService;
    private readonly IAlumnoApiService _alumnoService;
    private readonly IAlumnoSuscripcionApiService _alumnoSuscripcionService;
    private readonly IActividadApiService _actividadService;

    public SuscripcionesController(
        ISuscripcionApiService suscripcionService,
        IAlumnoApiService alumnoService,
        IAlumnoSuscripcionApiService alumnoSuscripcionService,
        IActividadApiService actividadService)
    {
        _suscripcionService = suscripcionService;
        _alumnoService = alumnoService;
        _alumnoSuscripcionService = alumnoSuscripcionService;
        _actividadService = actividadService;
    }

    public async Task<IActionResult> Index()
    {
        // Cantidad de clientes por plan, para mostrarla en cada tarjeta
        // (el listado de clientes ya trae el nombre del plan que tiene cada uno)
        var alumnos = await _alumnoService.ObtenerTodosAsync();
        ViewBag.ClientesPorPlan = alumnos.GroupBy(a => a.Suscripcion).ToDictionary(g => g.Key, g => g.Count());

        return View(await _suscripcionService.ObtenerTodasAsync());
    }

    // GET /Suscripciones/Alumnos/5 -> clientes que tienen este plan activo y las actividades de cada uno
    public async Task<IActionResult> Alumnos(int id)
    {
        var plan = await _suscripcionService.ObtenerPorIdAsync(id);
        if (plan == null) return NotFound();

        // 1) Clientes cuyo plan activo es este
        var alumnosDelPlan = new List<AlumnoDto>();
        foreach (var alumno in await _alumnoService.ObtenerTodosAsync())
        {
            var activa = (await _alumnoSuscripcionService.ObtenerHistorialAsync(alumno.Id)).FirstOrDefault(s => s.Activa);
            if (activa?.SuscripcionId == id)
                alumnosDelPlan.Add(alumno);
        }

        // 2) Actividades de cada cliente: se recorre cada actividad una sola vez
        //    y se anota en qué actividades está inscripto cada alumno
        var actividadesPorAlumno = new Dictionary<int, List<string>>();
        foreach (var actividad in await _actividadService.ObtenerTodasAsync())
        {
            foreach (var inscripto in await _actividadService.ObtenerAlumnosInscriptosAsync(actividad.ActividadId))
            {
                if (!actividadesPorAlumno.ContainsKey(inscripto.AlumnoId))
                    actividadesPorAlumno[inscripto.AlumnoId] = new List<string>();
                actividadesPorAlumno[inscripto.AlumnoId].Add(actividad.Nombre);
            }
        }

        ViewBag.Plan = plan;
        ViewBag.ActividadesPorAlumno = actividadesPorAlumno;
        return View(alumnosDelPlan.OrderBy(a => a.Apellido).ThenBy(a => a.Nombre).ToList());
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SuscripcionCreateDto dto)
    {
        var (ok, error) = await _suscripcionService.CrearAsync(dto);
        if (!ok)
        {
            ViewBag.Error = error;
            return View();
        }

        TempData["Mensaje"] = "Suscripción registrada con éxito en el sistema";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var suscripcion = await _suscripcionService.ObtenerPorIdAsync(id);
        if (suscripcion == null) return NotFound();
        return View(suscripcion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SuscripcionCreateDto dto)
    {
        var ok = await _suscripcionService.ActualizarAsync(id, dto);
        if (!ok)
        {
            ViewBag.Error = "No se pudo modificar la suscripción";
            return View(await _suscripcionService.ObtenerPorIdAsync(id));
        }

        TempData["Mensaje"] = "Los datos de la suscripción se han modificado con éxito en el sistema";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _suscripcionService.EliminarAsync(id);
        if (ok)
            TempData["Mensaje"] = "La suscripción se ha eliminado con éxito del sistema";
        else
            TempData["Error"] = "No se pudo eliminar la suscripción. Revisá que ningún cliente tenga o haya tenido ese plan.";
        return RedirectToAction("Index");
    }
}
