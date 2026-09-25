using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Profesor")]
public class ProfesorPanelController : Controller
{
    private readonly IActividadApiService _actividadService;
    private readonly IRutinaApiService _rutinaService;
    private readonly IAlimentacionApiService _alimentacionService;

    public ProfesorPanelController(IActividadApiService actividadService, IRutinaApiService rutinaService, IAlimentacionApiService alimentacionService)
    {
        _actividadService = actividadService;
        _rutinaService = rutinaService;
        _alimentacionService = alimentacionService;
    }

    private int ProfesorId => int.Parse(HttpContext.Session.GetString("UserId")!);

    public async Task<IActionResult> Index()
    {
        var actividades = await _actividadService.ObtenerTodasAsync();
        ViewBag.Actividades = actividades.Where(a => a.ProfesorId == ProfesorId).ToList();
        ViewBag.Rutinas = await _rutinaService.ObtenerDeProfesorAsync(ProfesorId);
        return View();
    }

    public async Task<IActionResult> Actividades()
    {
        var actividades = (await _actividadService.ObtenerTodasAsync())
            .Where(a => a.ProfesorId == ProfesorId)
            .ToList();
        return View(actividades);
    }

    public async Task<IActionResult> Rutinas()
    {
        return View(await _rutinaService.ObtenerDeProfesorAsync(ProfesorId));
    }

    public async Task<IActionResult> CrearRutina(int actividadId)
    {
        var actividad = await _actividadService.ObtenerPorIdAsync(actividadId);
        if (actividad is null || actividad.ProfesorId != ProfesorId) return Forbid();

        ViewBag.Actividad = actividad;
        ViewBag.Alumnos = await _actividadService.ObtenerAlumnosInscriptosAsync(actividadId);
        return View();
    }

    // La misma rutina se puede asignar a varios alumnos de la actividad:
    // se guarda una copia por cada alumno seleccionado.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearRutina(int actividadId, List<int> alumnoIds, string nombre, string descripcion)
    {
        var actividad = await _actividadService.ObtenerPorIdAsync(actividadId);
        if (actividad is null || actividad.ProfesorId != ProfesorId) return Forbid();

        string? error = alumnoIds.Count == 0 ? "Seleccioná al menos un alumno." : null;

        foreach (var alumnoId in alumnoIds)
        {
            var (ok, errorApi) = await _rutinaService.CrearAsync(new(nombre, descripcion, ProfesorId, alumnoId, actividadId));
            if (!ok) { error = errorApi; break; }
        }

        if (error == null)
        {
            TempData["Mensaje"] = alumnoIds.Count == 1
                ? "Rutina asignada con éxito"
                : $"Rutina asignada con éxito a {alumnoIds.Count} alumnos";
            return RedirectToAction(nameof(Rutinas));
        }

        ViewBag.Error = error;
        ViewBag.Actividad = actividad;
        ViewBag.Alumnos = await _actividadService.ObtenerAlumnosInscriptosAsync(actividadId);
        return View();
    }

    public async Task<IActionResult> AlumnosActividad(int id)
    {
        var actividad = await _actividadService.ObtenerPorIdAsync(id);
        if (actividad is null || actividad.ProfesorId != ProfesorId) return Forbid();
        ViewBag.Actividad = actividad;
        return View(await _actividadService.ObtenerAlumnosInscriptosAsync(id));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuitarAlumno(int actividadId, int alumnoId)
    {
        var actividad = await _actividadService.ObtenerPorIdAsync(actividadId);
        if (actividad is null || actividad.ProfesorId != ProfesorId) return Forbid();

        var ok = await _actividadService.DarDeBajaAlumnoAsync(alumnoId, actividadId);
        if (ok)
            TempData["Mensaje"] = "El alumno fue dado de baja de la actividad.";
        else
            TempData["Error"] = "No se pudo dar de baja al alumno de la actividad.";
        return RedirectToAction(nameof(AlumnosActividad), new { id = actividadId });
    }

    public async Task<IActionResult> Alimentaciones()
    {
        var planes = (await _alimentacionService.ObtenerTodasAsync()).Where(a => a.ProfesorId == ProfesorId).ToList();
        return View(planes);
    }

    public async Task<IActionResult> CrearAlimentacion(int? alumnoId)
    {
        await CargarAlumnosAsignablesAsync();
        ViewBag.AlumnoPreseleccionado = alumnoId;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearAlimentacion(string tipoAlimentacion, string descripcion, int alumnoId)
    {
        var (ok, error) = await _alimentacionService.CrearAsync(new(tipoAlimentacion, descripcion, ProfesorId, alumnoId));
        if (ok)
        {
            TempData["Mensaje"] = "Plan de alimentación asignado con éxito";
            return RedirectToAction(nameof(Alimentaciones));
        }
        ViewBag.Error = error;
        await CargarAlumnosAsignablesAsync();
        return View();
    }

    private async Task CargarAlumnosAsignablesAsync()
    {
        var actividades = (await _actividadService.ObtenerTodasAsync()).Where(a => a.ProfesorId == ProfesorId);
        var alumnos = new Dictionary<int, AlumnoInscriptoDto>();
        foreach (var actividad in actividades)
            foreach (var alumno in await _actividadService.ObtenerAlumnosInscriptosAsync(actividad.ActividadId))
                alumnos[alumno.AlumnoId] = alumno;
        ViewBag.Alumnos = alumnos.Values.OrderBy(a => a.Apellido).ThenBy(a => a.Nombre).ToList();
    }
}
