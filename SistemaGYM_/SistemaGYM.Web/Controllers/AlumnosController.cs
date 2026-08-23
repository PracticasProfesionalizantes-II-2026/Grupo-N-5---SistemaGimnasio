using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

public class AlumnosController : Controller
{
    private readonly IAlumnoApiService _alumnoApiService;

    public AlumnosController(IAlumnoApiService alumnoApiService)
    {
        _alumnoApiService = alumnoApiService;
    }

    // GET /Alumnos
    public async Task<IActionResult> Index()
    {
        var alumnos = await _alumnoApiService.ObtenerTodosAsync();
        return View(alumnos);
    }

    // GET /Alumnos/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var detalle = await _alumnoApiService.ObtenerDetalleAsync(id);
        if (detalle == null) return NotFound();

        return View(detalle);
    }
}
