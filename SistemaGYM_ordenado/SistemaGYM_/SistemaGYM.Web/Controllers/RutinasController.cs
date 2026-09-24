using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

// Admin: gestiona todas las rutinas.
// Profesor: modifica y elimina solo las rutinas que creó él (las crea desde su panel).
// Alumno: solo ve sus rutinas.
[SessionAuthorize("Administrador", "Alumno", "Profesor")]
public class RutinasController : Controller
{
    private readonly IRutinaApiService _rutinaService;
    private readonly IAlumnoApiService _alumnoService;
    private readonly IProfesorApiService _profesorService;

    public RutinasController(IRutinaApiService rutinaService, IAlumnoApiService alumnoService, IProfesorApiService profesorService)
    {
        _rutinaService = rutinaService;
        _alumnoService = alumnoService;
        _profesorService = profesorService;
    }

    private string? Rol => HttpContext.Session.GetString("Rol");
    private bool EsAdmin => Rol == "Administrador";
    private bool EsProfesor => Rol == "Profesor";
    private int UsuarioId => int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

    // El admin puede tocar cualquier rutina; el profesor solo las suyas
    private bool PuedeGestionar(RutinaDto rutina) => EsAdmin || (EsProfesor && rutina.ProfesorId == UsuarioId);

    // A dónde volver después de modificar o eliminar
    private IActionResult VolverAlListado() =>
        EsProfesor ? RedirectToAction("Rutinas", "ProfesorPanel") : RedirectToAction("Index");

    // GET /Rutinas -> gestión completa (solo Admin)
    public async Task<IActionResult> Index()
    {
        if (EsProfesor) return RedirectToAction("Rutinas", "ProfesorPanel");
        if (!EsAdmin) return RedirectToAction("MisRutinas");

        ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
        return View(await _rutinaService.ObtenerTodasAsync());
    }

    // GET /Rutinas/MisRutinas -> vista del Cliente
    public async Task<IActionResult> MisRutinas()
    {
        if (Rol != "Alumno") return RedirectToAction("Index");
        return View(await _rutinaService.ObtenerDeAlumnoAsync(UsuarioId));
    }

    private async Task CargarCombosAsync()
    {
        ViewBag.Alumnos = await _alumnoService.ObtenerTodosAsync();
        ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
    }

    public async Task<IActionResult> Create()
    {
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");
        await CargarCombosAsync();
        return View();
    }

    // Una misma rutina se puede asignar a varios alumnos a la vez:
    // se guarda una copia por cada alumno seleccionado.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string nombre, string descripcion, int profesorId, List<int> alumnoIds)
    {
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");

        if (alumnoIds.Count == 0)
        {
            ViewBag.Error = "Seleccioná al menos un alumno.";
            await CargarCombosAsync();
            return View();
        }

        foreach (var alumnoId in alumnoIds)
        {
            var (ok, error) = await _rutinaService.CrearAsync(new RutinaCreateDto(nombre, descripcion, profesorId, alumnoId, null));
            if (!ok)
            {
                ViewBag.Error = error;
                await CargarCombosAsync();
                return View();
            }
        }

        TempData["Mensaje"] = alumnoIds.Count == 1
            ? "Rutina registrada con éxito en el sistema"
            : $"Rutina asignada con éxito a {alumnoIds.Count} alumnos";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var rutina = (await _rutinaService.ObtenerTodasAsync()).FirstOrDefault(r => r.Id == id);
        if (rutina == null) return NotFound();
        if (!PuedeGestionar(rutina)) return RedirectToAction("AccesoDenegado", "Auth");

        await CargarCombosAsync();
        return View(rutina);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RutinaCreateDto dto)
    {
        var rutina = (await _rutinaService.ObtenerTodasAsync()).FirstOrDefault(r => r.Id == id);
        if (rutina == null) return NotFound();
        if (!PuedeGestionar(rutina)) return RedirectToAction("AccesoDenegado", "Auth");

        // Un profesor no puede pasarle su rutina a otro profesor
        if (EsProfesor) dto = dto with { ProfesorId = UsuarioId };

        var (ok, error) = await _rutinaService.ActualizarAsync(id, dto);
        if (!ok)
        {
            ViewBag.Error = error;
            await CargarCombosAsync();
            return View(rutina);
        }

        TempData["Mensaje"] = "Los datos de la rutina se han modificado con éxito en el sistema";
        return VolverAlListado();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var rutina = (await _rutinaService.ObtenerTodasAsync()).FirstOrDefault(r => r.Id == id);
        if (rutina == null) return NotFound();
        if (!PuedeGestionar(rutina)) return RedirectToAction("AccesoDenegado", "Auth");

        await _rutinaService.EliminarAsync(id);
        TempData["Mensaje"] = "La rutina se ha eliminado con éxito del sistema";
        return VolverAlListado();
    }
}
