using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Alumno")]
public class ClasesController : Controller
{
    private readonly IActividadApiService _actividadService;
    private readonly IProfesorApiService _profesorService;

    public ClasesController(IActividadApiService actividadService, IProfesorApiService profesorService)
    {
        _actividadService = actividadService;
        _profesorService = profesorService;
    }

    private int AlumnoId => int.Parse(HttpContext.Session.GetString("UserId")!);

    // GET /Clases -> Mis Clases
    public async Task<IActionResult> Index()
    {
        var misActividades = await _actividadService.ObtenerActividadesDeAlumnoAsync(AlumnoId);
        return View(misActividades);
    }

    // GET /Clases/Todas -> lista completa para inscribirse
    public async Task<IActionResult> Todas()
    {
        var todas = await _actividadService.ObtenerTodasAsync();
        return View(todas);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inscribirse(int actividadId)
    {
        var (ok, error) = await _actividadService.InscribirAlumnoAsync(AlumnoId, actividadId);
        TempData["Mensaje"] = ok ? "Te inscribiste correctamente a la actividad" : error;
        return RedirectToAction("Todas");
    }

    // GET /Clases/Detalle/5 -> ver profesor + alumnos anotados
    public async Task<IActionResult> Detalle(int id)
    {
        var actividad = await _actividadService.ObtenerPorIdAsync(id);
        if (actividad == null) return NotFound();

        var profesor = await _profesorService.ObtenerDetalleAsync(actividad.ProfesorId);
        var inscriptos = await _actividadService.ObtenerAlumnosInscriptosAsync(id);

        ViewBag.Actividad = actividad;
        ViewBag.Profesor = profesor;
        ViewBag.CantidadAlumnos = inscriptos.Count(i => i.Activa);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DarDeBaja(int actividadId)
    {
        await _actividadService.DarDeBajaAlumnoAsync(AlumnoId, actividadId);
        TempData["Mensaje"] = "Te diste de baja de la actividad";
        return RedirectToAction("Index");
    }
}
