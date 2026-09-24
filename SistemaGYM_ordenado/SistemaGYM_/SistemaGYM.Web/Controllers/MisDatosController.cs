using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Alumno")]
public class MisDatosController : Controller
{
    private readonly IAlumnoApiService _alumnoService;

    public MisDatosController(IAlumnoApiService alumnoService)
    {
        _alumnoService = alumnoService;
    }

    private int AlumnoId => int.Parse(HttpContext.Session.GetString("UserId")!);

    public async Task<IActionResult> Index()
    {
        var detalle = await _alumnoService.ObtenerDetalleAsync(AlumnoId);
        if (detalle == null) return NotFound();
        return View(detalle);
    }

    public async Task<IActionResult> Editar()
    {
        var detalle = await _alumnoService.ObtenerDetalleAsync(AlumnoId);
        if (detalle == null) return NotFound();
        return View(detalle);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(AlumnoCreateDto dto)
    {
        var (ok, error) = await _alumnoService.ActualizarAsync(AlumnoId, dto);
        if (!ok)
        {
            ViewBag.Error = error;
            return View(await _alumnoService.ObtenerDetalleAsync(AlumnoId));
        }

        HttpContext.Session.SetString("Nombre", dto.Nombre);
        HttpContext.Session.SetString("Apellido", dto.Apellido);

        TempData["Mensaje"] = "Los datos del cliente se han modificado con éxito en el sistema";
        return RedirectToAction("Index");
    }
}
