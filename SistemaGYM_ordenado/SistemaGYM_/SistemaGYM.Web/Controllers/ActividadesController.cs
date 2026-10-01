using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Administrador")]
public class ActividadesController : Controller
{
    private readonly IActividadApiService _actividadService;
    private readonly IProfesorApiService _profesorService;
    private readonly IAlumnoApiService _alumnoService;

    public ActividadesController(IActividadApiService actividadService, IProfesorApiService profesorService, IAlumnoApiService alumnoService)
    {
        _actividadService = actividadService;
        _profesorService = profesorService;
        _alumnoService = alumnoService;
    }

    public async Task<IActionResult> Index()
    {
        await CargarProfesoresAsync(); // para mostrar el nombre del profesor en la tabla
        return View(await _actividadService.ObtenerTodasAsync());
    }

    // Solo se listan profesores activos: una actividad no puede quedar a cargo de otro tipo de usuario
    private async Task CargarProfesoresAsync()
    {
        ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
    }

    public async Task<IActionResult> Create()
    {
        await CargarProfesoresAsync();
        ViewBag.Alumnos = await _alumnoService.ObtenerTodosAsync();
        return View();
    }

    // Los alumnos son opcionales: la actividad se puede crear vacía e inscribirlos después.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string nombre, string descripcion, TimeOnly horaInicio, TimeOnly horaFin, int profesorId, int cupo, List<string> dias, List<int> alumnoIds)
    {
        var dto = new ActividadCreateDto(nombre, descripcion, horaInicio, horaFin, profesorId, CombinarDias(dias), cupo);
        var (ok, error, creada) = await _actividadService.CrearAsync(dto);

        if (!ok || creada is null)
        {
            ViewBag.Error = error;
            await CargarProfesoresAsync();
            ViewBag.Alumnos = await _alumnoService.ObtenerTodosAsync();
            return View();
        }

        var (_, errores) = await _actividadService.InscribirAlumnosAsync(creada.ActividadId, alumnoIds);
        if (errores.Any())
            TempData["Error"] = $"La actividad se registró, pero algunos alumnos no se pudieron inscribir: {string.Join(" ", errores)}";
        else
            TempData["Mensaje"] = "Actividad registrada con éxito en el sistema";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var actividad = await _actividadService.ObtenerPorIdAsync(id);
        if (actividad == null) return NotFound();
        await CargarProfesoresAsync();
        return View(actividad);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string nombre, string descripcion, TimeOnly horaInicio, TimeOnly horaFin, int profesorId, int cupo, List<string> dias)
    {
        var dto = new ActividadCreateDto(nombre, descripcion, horaInicio, horaFin, profesorId, CombinarDias(dias), cupo);
        var (ok, error) = await _actividadService.ActualizarAsync(id, dto);

        if (!ok)
        {
            ViewBag.Error = error;
            await CargarProfesoresAsync();
            return View(await _actividadService.ObtenerPorIdAsync(id));
        }

        TempData["Mensaje"] = "Actividad modificada con éxito en el sistema";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var (ok, error) = await _actividadService.EliminarAsync(id);
        if (ok)
            TempData["Mensaje"] = "La actividad se ha eliminado con éxito del sistema";
        else
            TempData["Error"] = error;
        return RedirectToAction("Index");
    }

    // GET /Actividades/Alumnos/5 -> ver alumnos inscriptos e inscribir nuevos
    public async Task<IActionResult> Alumnos(int id)
    {
        var actividad = await _actividadService.ObtenerPorIdAsync(id);
        if (actividad == null) return NotFound();

        var inscriptos = await _actividadService.ObtenerAlumnosInscriptosAsync(id);
        ViewBag.Actividad = actividad;
        ViewBag.AlumnosDisponibles = await AlumnosNoInscriptosAsync(inscriptos);
        return View(inscriptos);
    }

    // POST /Actividades/InscribirAlumnos -> el admin inscribe a uno o varios alumnos
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InscribirAlumnos(int actividadId, List<int> alumnoIds)
    {
        if (alumnoIds.Count == 0)
        {
            TempData["Error"] = "Seleccioná al menos un alumno.";
            return RedirectToAction(nameof(Alumnos), new { id = actividadId });
        }

        var (inscriptos, errores) = await _actividadService.InscribirAlumnosAsync(actividadId, alumnoIds);
        if (errores.Any())
            TempData["Error"] = $"Se inscribieron {inscriptos} de {alumnoIds.Count} alumnos. {string.Join(" ", errores)}";
        else
            TempData["Mensaje"] = inscriptos == 1 ? "Alumno inscripto con éxito" : $"{inscriptos} alumnos inscriptos con éxito";
        return RedirectToAction(nameof(Alumnos), new { id = actividadId });
    }

    // Clientes activos que todavía no están inscriptos en la actividad
    private async Task<List<AlumnoDto>> AlumnosNoInscriptosAsync(List<AlumnoInscriptoDto> inscriptos)
    {
        var idsInscriptos = inscriptos.Where(i => i.Activa).Select(i => i.AlumnoId).ToHashSet();
        return (await _alumnoService.ObtenerTodosAsync()).Where(a => !idsInscriptos.Contains(a.Id)).ToList();
    }

    // POST /Actividades/QuitarAlumno -> el admin da de baja a un alumno de la actividad
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuitarAlumno(int actividadId, int alumnoId)
    {
        var (ok, error) = await _actividadService.DarDeBajaAlumnoAsync(alumnoId, actividadId);
        if (ok)
            TempData["Mensaje"] = "El alumno fue dado de baja de la actividad.";
        else
            TempData["Error"] = error;
        return RedirectToAction(nameof(Alumnos), new { id = actividadId });
    }

    private static DiasSemana CombinarDias(List<string>? dias)
    {
        DiasSemana resultado = 0;
        if (dias == null) return resultado;
        foreach (var d in dias)
            if (Enum.TryParse<DiasSemana>(d, out var parsed))
                resultado |= parsed;
        return resultado;
    }
}
