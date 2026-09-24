using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

// Admin: ABM de todos los planes.
// Profesor: modifica y elimina solo los planes que creó él (los crea desde su panel).
// Alumno: ve sus planes y los planes generales.
[SessionAuthorize("Administrador", "Alumno", "Profesor")]
public class AlimentacionController : Controller
{
    private readonly IAlimentacionApiService _alimentacionService;
    private readonly IProfesorApiService _profesorService;

    public AlimentacionController(IAlimentacionApiService alimentacionService, IProfesorApiService profesorService)
    {
        _alimentacionService = alimentacionService;
        _profesorService = profesorService;
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

        // El alumno solo ve los planes asignados a él y los planes generales (sin alumno)
        if (!EsAdmin)
            planes = planes.Where(p => p.AlumnoId == null || p.AlumnoId == UsuarioId).ToList();

        return View(planes);
    }

    public async Task<IActionResult> Details(int id)
    {
        var plan = await _alimentacionService.ObtenerPorIdAsync(id);
        if (plan == null) return NotFound();
        ViewBag.PuedeGestionar = PuedeGestionar(plan);
        return View(plan);
    }

    public async Task<IActionResult> Create()
    {
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");
        ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlimentacionCreateDto dto)
    {
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");

        var (ok, error) = await _alimentacionService.CrearAsync(dto);
        if (!ok)
        {
            ViewBag.Error = error;
            ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
            return View();
        }

        TempData["Mensaje"] = "Plan de alimentación registrado con éxito en el sistema";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var plan = await _alimentacionService.ObtenerPorIdAsync(id);
        if (plan == null) return NotFound();
        if (!PuedeGestionar(plan)) return RedirectToAction("AccesoDenegado", "Auth");

        ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
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
            ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
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

        await _alimentacionService.EliminarAsync(id);
        TempData["Mensaje"] = "El plan de alimentación se eliminó con éxito del sistema";
        return VolverAlListado();
    }
}
