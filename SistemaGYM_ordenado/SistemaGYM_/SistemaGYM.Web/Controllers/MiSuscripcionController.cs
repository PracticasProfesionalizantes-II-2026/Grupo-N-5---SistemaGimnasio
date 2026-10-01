using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Alumno")]
public class MiSuscripcionController : Controller
{
    private readonly IAlumnoSuscripcionApiService _alumnoSuscripcionService;
    private readonly ISuscripcionApiService _suscripcionService;

    public MiSuscripcionController(IAlumnoSuscripcionApiService alumnoSuscripcionService, ISuscripcionApiService suscripcionService)
    {
        _alumnoSuscripcionService = alumnoSuscripcionService;
        _suscripcionService = suscripcionService;
    }

    private int AlumnoId => int.Parse(HttpContext.Session.GetString("UserId")!);

    public async Task<IActionResult> Index()
    {
        var historial = await _alumnoSuscripcionService.ObtenerHistorialAsync(AlumnoId);
        var activa = historial.FirstOrDefault(s => s.Activa);

        if (activa == null)
            ViewBag.Planes = await _suscripcionService.ObtenerTodasAsync();

        return View(activa);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Asignar(int suscripcionId)
    {
        var (ok, error) = await _alumnoSuscripcionService.AsignarAsync(AlumnoId, suscripcionId);
        if (ok)
            TempData["Mensaje"] = "Se ha registrado la suscripción con éxito";
        else
            TempData["Error"] = error;
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int alumnoSuscripcionId)
    {
        var (ok, error) = await _alumnoSuscripcionService.CancelarAsync(AlumnoId, alumnoSuscripcionId);
        if (ok)
            TempData["Mensaje"] = "Suscripción cancelada";
        else
            TempData["Error"] = error;
        return RedirectToAction("Index");
    }
}
