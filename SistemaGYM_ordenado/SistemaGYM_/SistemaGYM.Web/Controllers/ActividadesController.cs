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

    public ActividadesController(IActividadApiService actividadService, IProfesorApiService profesorService)
    {
        _actividadService = actividadService;
        _profesorService = profesorService;
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
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string nombre, string descripcion, TimeOnly horaInicio, TimeOnly horaFin, int profesorId, int cupo, List<string> dias)
    {
        var dto = new ActividadCreateDto(nombre, descripcion, horaInicio, horaFin, profesorId, CombinarDias(dias), cupo);
        var (ok, error) = await _actividadService.CrearAsync(dto);

        if (!ok)
        {
            ViewBag.Error = error;
            await CargarProfesoresAsync();
            return View();
        }

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
        var ok = await _actividadService.EliminarAsync(id);
        if (ok)
            TempData["Mensaje"] = "La actividad se ha eliminado con éxito del sistema";
        else
            TempData["Error"] = "No se pudo eliminar la actividad. Revisá que no tenga rutinas asignadas.";
        return RedirectToAction("Index");
    }

    // GET /Actividades/Alumnos/5 -> ver alumnos inscriptos
    public async Task<IActionResult> Alumnos(int id)
    {
        var actividad = await _actividadService.ObtenerPorIdAsync(id);
        if (actividad == null) return NotFound();

        ViewBag.Actividad = actividad;
        return View(await _actividadService.ObtenerAlumnosInscriptosAsync(id));
    }

    // POST /Actividades/QuitarAlumno -> el admin da de baja a un alumno de la actividad
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuitarAlumno(int actividadId, int alumnoId)
    {
        var ok = await _actividadService.DarDeBajaAlumnoAsync(alumnoId, actividadId);
        if (ok)
            TempData["Mensaje"] = "El alumno fue dado de baja de la actividad.";
        else
            TempData["Error"] = "No se pudo dar de baja al alumno de la actividad.";
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
