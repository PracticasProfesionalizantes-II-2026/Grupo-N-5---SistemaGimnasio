using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Administrador")]
public class SuscripcionesController : Controller
{
    private readonly ISuscripcionApiService _suscripcionService;

    public SuscripcionesController(ISuscripcionApiService suscripcionService)
    {
        _suscripcionService = suscripcionService;
    }

    public async Task<IActionResult> Index() => View(await _suscripcionService.ObtenerTodasAsync());

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SuscripcionCreateDto dto)
    {
        var (ok, error) = await _suscripcionService.CrearAsync(dto);
        if (!ok)
        {
            ViewBag.Error = error;
            return View();
        }

        TempData["Mensaje"] = "Suscripción registrada con éxito en el sistema";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var suscripcion = await _suscripcionService.ObtenerPorIdAsync(id);
        if (suscripcion == null) return NotFound();
        return View(suscripcion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SuscripcionCreateDto dto)
    {
        var ok = await _suscripcionService.ActualizarAsync(id, dto);
        if (!ok)
        {
            ViewBag.Error = "No se pudo modificar la suscripción";
            return View(await _suscripcionService.ObtenerPorIdAsync(id));
        }

        TempData["Mensaje"] = "Los datos de la suscripción se han modificado con éxito en el sistema";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _suscripcionService.EliminarAsync(id);
        TempData["Mensaje"] = "La suscripción se ha eliminado con éxito del sistema";
        return RedirectToAction("Index");
    }
}
