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
    private readonly IAlumnoApiService _alumnoService;

    public ProfesorPanelController(IActividadApiService actividadService, IRutinaApiService rutinaService, IAlimentacionApiService alimentacionService, IAlumnoApiService alumnoService)
    {
        _actividadService = actividadService;
        _rutinaService = rutinaService;
        _alimentacionService = alimentacionService;
        _alumnoService = alumnoService;
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

    // Se guarda una sola rutina de la actividad, asignada a todos los alumnos elegidos
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearRutina(int actividadId, List<int> alumnoIds, string nombre, string descripcion)
    {
        var actividad = await _actividadService.ObtenerPorIdAsync(actividadId);
        if (actividad is null || actividad.ProfesorId != ProfesorId) return Forbid();

        string? error = alumnoIds.Count == 0 ? "Seleccioná al menos un alumno." : null;

        if (error == null)
        {
            var (ok, errorApi) = await _rutinaService.CrearAsync(new(nombre, descripcion, ProfesorId, alumnoIds, actividadId));
            if (ok)
            {
                TempData["Mensaje"] = alumnoIds.Count == 1
                    ? "Rutina asignada con éxito"
                    : $"Rutina asignada con éxito a {alumnoIds.Count} alumnos";
                return RedirectToAction(nameof(Rutinas));
            }
            error = errorApi;
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

        var inscriptos = await _actividadService.ObtenerAlumnosInscriptosAsync(id);
        var idsInscriptos = inscriptos.Where(i => i.Activa).Select(i => i.AlumnoId).ToHashSet();

        ViewBag.Actividad = actividad;
        // Clientes activos que todavía no están en la actividad (para el formulario de inscripción)
        ViewBag.AlumnosDisponibles = (await _alumnoService.ObtenerTodosAsync()).Where(a => !idsInscriptos.Contains(a.Id)).ToList();
        return View(inscriptos);
    }

    // El profesor puede inscribir alumnos, pero solo en sus propias actividades
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InscribirAlumnos(int actividadId, List<int> alumnoIds)
    {
        var actividad = await _actividadService.ObtenerPorIdAsync(actividadId);
        if (actividad is null || actividad.ProfesorId != ProfesorId) return Forbid();

        if (alumnoIds.Count == 0)
        {
            TempData["Error"] = "Seleccioná al menos un alumno.";
            return RedirectToAction(nameof(AlumnosActividad), new { id = actividadId });
        }

        var (inscriptos, errores) = await _actividadService.InscribirAlumnosAsync(actividadId, alumnoIds);
        if (errores.Any())
            TempData["Error"] = $"Se inscribieron {inscriptos} de {alumnoIds.Count} alumnos. {string.Join(" ", errores)}";
        else
            TempData["Mensaje"] = inscriptos == 1 ? "Alumno inscripto con éxito" : $"{inscriptos} alumnos inscriptos con éxito";
        return RedirectToAction(nameof(AlumnosActividad), new { id = actividadId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuitarAlumno(int actividadId, int alumnoId)
    {
        var actividad = await _actividadService.ObtenerPorIdAsync(actividadId);
        if (actividad is null || actividad.ProfesorId != ProfesorId) return Forbid();

        var (ok, error) = await _actividadService.DarDeBajaAlumnoAsync(alumnoId, actividadId);
        if (ok)
            TempData["Mensaje"] = "El alumno fue dado de baja de la actividad.";
        else
            TempData["Error"] = error;
        return RedirectToAction(nameof(AlumnosActividad), new { id = actividadId });
    }

    public async Task<IActionResult> Alimentaciones()
    {
        var planes = (await _alimentacionService.ObtenerTodasAsync()).Where(a => a.ProfesorId == ProfesorId).ToList();
        return View(planes);
    }

    // Los planes se crean desde Alimentacion/Create, el mismo formulario que usa el admin
}
