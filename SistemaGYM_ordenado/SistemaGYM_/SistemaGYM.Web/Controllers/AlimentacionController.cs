using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

// Admin: ABM de todos los planes.
// Profesor: crea planes a su nombre (mismo formulario que el admin) y modifica/elimina solo los suyos.
// Alumno: ve sus planes y los planes generales.
[SessionAuthorize("Administrador", "Alumno", "Profesor")]
public class AlimentacionController : Controller
{
    private readonly IAlimentacionApiService _alimentacionService;
    private readonly IProfesorApiService _profesorService;
    private readonly IAlumnoApiService _alumnoService;

    public AlimentacionController(IAlimentacionApiService alimentacionService, IProfesorApiService profesorService, IAlumnoApiService alumnoService)
    {
        _alimentacionService = alimentacionService;
        _profesorService = profesorService;
        _alumnoService = alumnoService;
    }

    private string? Rol => HttpContext.Session.GetString("Rol");
    private bool EsAdmin => Rol == "Administrador";
    private bool EsProfesor => Rol == "Profesor";
    private int UsuarioId => int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

    // El admin puede tocar cualquier plan; el profesor solo los suyos
    private bool PuedeGestionar(AlimentacionDto plan) => EsAdmin || (EsProfesor && plan.ProfesorId == UsuarioId);

    private IActionResult VolverAlListado() =>
        EsProfesor ? RedirectToAction("Alimentaciones", "ProfesorPanel") : RedirectToAction("Index");

    public async Task<IActionResult> Index()
    {
        if (EsProfesor) return RedirectToAction("Alimentaciones", "ProfesorPanel");

        var planes = await _alimentacionService.ObtenerTodasAsync();

        // El alumno solo ve los planes asignados a él y los planes generales (sin alumnos)
        if (!EsAdmin)
            planes = planes.Where(p => p.Alumnos.Count == 0 || p.Alumnos.Any(a => a.AlumnoId == UsuarioId)).ToList();

        return View(planes);
    }

    public async Task<IActionResult> Details(int id)
    {
        var plan = await _alimentacionService.ObtenerPorIdAsync(id);
        if (plan == null) return NotFound();
        ViewBag.PuedeGestionar = PuedeGestionar(plan);
        return View(plan);
    }

    private async Task CargarCombosAsync()
    {
        ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
        ViewBag.Alumnos = await _alumnoService.ObtenerTodosAsync();
    }

    // GET /Alimentacion/Create?alumnoId=5 -> el alumnoId es opcional y deja ese alumno ya marcado
    public async Task<IActionResult> Create(int? alumnoId)
    {
        if (!EsAdmin && !EsProfesor) return RedirectToAction("AccesoDenegado", "Auth");
        await CargarCombosAsync();
        ViewBag.AlumnosSeleccionados = alumnoId.HasValue ? new List<int> { alumnoId.Value } : new List<int>();
        return View();
    }

    // Se guarda un solo plan para todos los alumnos elegidos.
    // Elegir alumnos es opcional: sin alumnos es un plan general.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlimentacionCreateDto dto)
    {
        if (!EsAdmin && !EsProfesor) return RedirectToAction("AccesoDenegado", "Auth");

        // El profesor siempre crea el plan a su nombre
        if (EsProfesor) dto = dto with { ProfesorId = UsuarioId };

        var (ok, error) = await _alimentacionService.CrearAsync(dto);
        if (!ok)
        {
            ViewBag.Error = error;
            await CargarCombosAsync();
            ViewBag.AlumnosSeleccionados = dto.AlumnoIds;
            return View();
        }

        TempData["Mensaje"] = "Plan de alimentación registrado con éxito en el sistema";
        return VolverAlListado();
    }

    public async Task<IActionResult> Edit(int id)
    {
        var plan = await _alimentacionService.ObtenerPorIdAsync(id);
        if (plan == null) return NotFound();
        if (!PuedeGestionar(plan)) return RedirectToAction("AccesoDenegado", "Auth");

        await CargarCombosAsync();
        ViewBag.AlumnosSeleccionados = plan.Alumnos.Select(a => a.AlumnoId).ToList();
        return View(plan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AlimentacionCreateDto dto)
    {
        var plan = await _alimentacionService.ObtenerPorIdAsync(id);
        if (plan == null) return NotFound();
        if (!PuedeGestionar(plan)) return RedirectToAction("AccesoDenegado", "Auth");

        // El profesor no puede pasarle su plan a otro profesor
        if (EsProfesor) dto = dto with { ProfesorId = UsuarioId };

        var (ok, error) = await _alimentacionService.ActualizarAsync(id, dto);
        if (!ok)
        {
            ViewBag.Error = error;
            await CargarCombosAsync();
            ViewBag.AlumnosSeleccionados = dto.AlumnoIds;
            return View(plan);
        }

        TempData["Mensaje"] = "El plan de alimentación se modificó con éxito en el sistema";
        return VolverAlListado();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var plan = await _alimentacionService.ObtenerPorIdAsync(id);
        if (plan == null) return NotFound();
        if (!PuedeGestionar(plan)) return RedirectToAction("AccesoDenegado", "Auth");

        var (ok, error) = await _alimentacionService.EliminarAsync(id);
        if (ok)
            TempData["Mensaje"] = "El plan de alimentación se eliminó con éxito del sistema";
        else
            TempData["Error"] = error;
        return VolverAlListado();
    }
}
