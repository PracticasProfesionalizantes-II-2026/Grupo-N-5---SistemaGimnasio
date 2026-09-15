using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Administrador", "Alumno")]
public class AlimentacionController : Controller
{
    private readonly IAlimentacionApiService _alimentacionService;
    private readonly IProfesorApiService _profesorService;

    public AlimentacionController(IAlimentacionApiService alimentacionService, IProfesorApiService profesorService)
    {
        _alimentacionService = alimentacionService;
        _profesorService = profesorService;
    }

    private bool EsAdmin => HttpContext.Session.GetString("Rol") == "Administrador";

    public async Task<IActionResult> Index() => View(await _alimentacionService.ObtenerTodasAsync());

    public async Task<IActionResult> Details(int id)
    {
        var plan = await _alimentacionService.ObtenerPorIdAsync(id);
        if (plan == null) return NotFound();
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
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");

        var plan = await _alimentacionService.ObtenerPorIdAsync(id);
        if (plan == null) return NotFound();

        ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
        return View(plan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AlimentacionCreateDto dto)
    {
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");

        await _alimentacionService.ActualizarAsync(id, dto);
        TempData["Mensaje"] = "El plan de alimentación se modificó con éxito en el sistema";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");

        await _alimentacionService.EliminarAsync(id);
        TempData["Mensaje"] = "El plan de alimentación se eliminó con éxito del sistema";
        return RedirectToAction("Index");
    }
}
