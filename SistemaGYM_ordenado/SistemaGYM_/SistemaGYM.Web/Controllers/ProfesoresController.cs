using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Administrador")]
public class ProfesoresController : Controller
{
    private readonly IProfesorApiService _profesorService;
    private readonly IActividadApiService _actividadService;

    public ProfesoresController(IProfesorApiService profesorService, IActividadApiService actividadService)
    {
        _profesorService = profesorService;
        _actividadService = actividadService;
    }

    // GET /Profesores?buscar=ana&actividadId=3
    // Los dos filtros son opcionales y se pueden combinar.
    public async Task<IActionResult> Index(string? buscar, int? actividadId)
    {
        var profesores = await _profesorService.ObtenerTodosAsync();
        var actividades = await _actividadService.ObtenerTodasAsync();

        // Texto libre: busca en nombre, apellido, DNI, email o descripción
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim().ToLower();
            profesores = profesores.Where(p =>
                $"{p.Nombre} {p.Apellido}".ToLower().Contains(texto) ||
                p.Dni.ToString().Contains(texto) ||
                (p.Email ?? "").ToLower().Contains(texto) ||
                (p.Descripcion ?? "").ToLower().Contains(texto)).ToList();
        }

        // Profesor a cargo de una actividad en particular
        if (actividadId.HasValue)
        {
            var actividad = actividades.FirstOrDefault(a => a.ActividadId == actividadId.Value);
            profesores = profesores.Where(p => p.Id == actividad?.ProfesorId).ToList();
        }

        ViewBag.Buscar = buscar;
        ViewBag.ActividadId = actividadId;
        ViewBag.Actividades = actividades;

        return View(profesores.OrderBy(p => p.Apellido).ThenBy(p => p.Nombre).ToList());
    }

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
            return View(dto); // se devuelven los datos para no tener que cargarlos de nuevo
        }

        var (ok, error) = await _profesorService.CrearAsync(dto);
        if (!ok)
        {
            ViewBag.Error = error;
            return View(dto); // por ejemplo, DNI repetido: se conserva lo que había escrito
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
            TempData["Error"] = "No se pudo dar de baja al profesor. Revisá que no tenga actividades a cargo.";
        return RedirectToAction("Index");
    }
}
