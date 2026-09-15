using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Administrador")]
public class ReportesController : Controller
{
    private readonly IAlumnoApiService _alumnoService;
    private readonly IAlumnoSuscripcionApiService _alumnoSuscripcionService;

    public ReportesController(IAlumnoApiService alumnoService, IAlumnoSuscripcionApiService alumnoSuscripcionService)
    {
        _alumnoService = alumnoService;
        _alumnoSuscripcionService = alumnoSuscripcionService;
    }

    // GET /Reportes -> menú de reportes
    public IActionResult Index() => View();

    // GET /Reportes/Suscripciones?soloActivas=true|false
    public async Task<IActionResult> Suscripciones(bool soloActivas = true)
    {
        var alumnos = await _alumnoService.ObtenerTodosAsync();
        var filas = new List<(AlumnoDto Alumno, AlumnoSuscripcionDto Suscripcion)>();

        foreach (var alumno in alumnos)
        {
            var historial = await _alumnoSuscripcionService.ObtenerHistorialAsync(alumno.Id);
            foreach (var s in historial.Where(s => s.Activa == soloActivas))
                filas.Add((alumno, s));
        }

        ViewBag.SoloActivas = soloActivas;
        return View(filas);
    }
}
