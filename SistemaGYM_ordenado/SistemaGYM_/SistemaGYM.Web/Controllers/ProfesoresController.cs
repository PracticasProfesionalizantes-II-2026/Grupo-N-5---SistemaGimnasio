using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Administrador")]
public class ProfesoresController : Controller
{
    private readonly IProfesorApiService _profesorService;

    public ProfesoresController(IProfesorApiService profesorService)
    {
        _profesorService = profesorService;
    }

    public async Task<IActionResult> Index() => View(await _profesorService.ObtenerTodosAsync());

    public async Task<IActionResult> Details(int id)
    {
        var detalle = await _profesorService.ObtenerDetalleAsync(id);
        if (detalle == null) return NotFound();
        return View(detalle);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProfesorCreateDto dto, string confirmarContrasenia)
    {
        if (dto.Contrasenia != confirmarContrasenia)
        {
            ViewBag.Error = "Las contraseñas no coinciden.";
            return View();
        }

        var (ok, error) = await _profesorService.CrearAsync(dto);
        if (!ok)
        {
            ViewBag.Error = error;
            return View();
        }

        TempData["Mensaje"] = "Profesor registrado con éxito en el sistema";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var detalle = await _profesorService.ObtenerDetalleAsync(id);
        if (detalle == null) return NotFound();
        return View(detalle);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProfesorCreateDto dto)
    {
        var (ok, error) = await _profesorService.ActualizarAsync(id, dto);
        if (!ok)
        {
            ViewBag.Error = error;
            return View(await _profesorService.ObtenerDetalleAsync(id));
        }

        TempData["Mensaje"] = "Los datos del profesor se han modificado con éxito en el sistema";
        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var eliminado = await _profesorService.EliminarAsync(id);
        if (eliminado)
            TempData["Mensaje"] = "El profesor se ha dado de baja correctamente del sistema";
        else
            TempData["Error"] = "No se pudo dar de baja al profesor.";
        return RedirectToAction("Index");
    }
}
