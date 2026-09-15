using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Administrador")]
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

    // GET /Alumnos/Create
    public IActionResult Create() => View();

    // POST /Alumnos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlumnoCreateDto dto)
    {
        var creado = await _alumnoApiService.CrearAsync(dto);
        if (!creado)
        {
            ViewBag.Error = "No se pudo registrar el cliente. Verificá que los datos no estén vacíos ni repetidos.";
            return View();
        }

        TempData["Mensaje"] = "Cliente registrado con éxito en el sistema";
        return RedirectToAction("Index");
    }

    // GET /Alumnos/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var detalle = await _alumnoApiService.ObtenerDetalleAsync(id);
        if (detalle == null) return NotFound();

        return View(detalle);
    }

    // POST /Alumnos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AlumnoCreateDto dto)
    {
        var actualizado = await _alumnoApiService.ActualizarAsync(id, dto);
        if (!actualizado)
        {
            ViewBag.Error = "No se pudo modificar el cliente";
            return View(await _alumnoApiService.ObtenerDetalleAsync(id));
        }

        TempData["Mensaje"] = "Los datos del cliente se han modificado con éxito en el sistema";
        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var eliminado = await _alumnoApiService.EliminarAsync(id);
        TempData["Mensaje"] = eliminado
            ? "El cliente se ha dado de baja correctamente del sistema"
            : "No se pudo dar de baja al cliente.";
        return RedirectToAction("Index");
    }
}
