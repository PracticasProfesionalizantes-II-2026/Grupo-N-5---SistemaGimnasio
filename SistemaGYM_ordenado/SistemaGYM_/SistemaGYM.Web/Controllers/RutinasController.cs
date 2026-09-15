using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Administrador", "Alumno")]
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

    private bool EsAdmin => HttpContext.Session.GetString("Rol") == "Administrador";
    private int AlumnoId => int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

    // GET /Rutinas -> gestión completa (solo Admin)
    public async Task<IActionResult> Index()
    {
        if (!EsAdmin) return RedirectToAction("MisRutinas");
        return View(await _rutinaService.ObtenerTodasAsync());
    }

    // GET /Rutinas/MisRutinas -> vista del Cliente
    public async Task<IActionResult> MisRutinas()
    {
        if (EsAdmin) return RedirectToAction("Index");
        return View(await _rutinaService.ObtenerDeAlumnoAsync(AlumnoId));
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RutinaCreateDto dto)
    {
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");

        var (ok, error) = await _rutinaService.CrearAsync(dto);
        if (!ok)
        {
            ViewBag.Error = error;
            await CargarCombosAsync();
            return View();
        }

        TempData["Mensaje"] = "Rutina registrada con éxito en el sistema";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");

        var rutina = (await _rutinaService.ObtenerTodasAsync()).FirstOrDefault(r => r.Id == id);
        if (rutina == null) return NotFound();

        await CargarCombosAsync();
        return View(rutina);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RutinaCreateDto dto)
    {
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");

        var ok = await _rutinaService.ActualizarAsync(id, dto);
        TempData["Mensaje"] = ok ? "Los datos de la actividad se han modificado con éxito en el sistema" : "No se pudo modificar la rutina";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");

        await _rutinaService.EliminarAsync(id);
        TempData["Mensaje"] = "La rutina se ha eliminado con éxito del sistema";
        return RedirectToAction("Index");
    }
}
