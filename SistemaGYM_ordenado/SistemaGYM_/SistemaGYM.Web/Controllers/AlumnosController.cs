using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

// El Profesor puede ver el listado y el detalle de los clientes.
// Registrar, modificar y dar de baja queda solo para el Administrador (ver cada acción).
[SessionAuthorize("Administrador", "Profesor")]
public class AlumnosController : Controller
{
    private readonly IAlumnoApiService _alumnoApiService;
    private readonly ISuscripcionApiService _suscripcionService;
    private readonly IAlumnoSuscripcionApiService _alumnoSuscripcionService;

    public AlumnosController(
        IAlumnoApiService alumnoApiService,
        ISuscripcionApiService suscripcionService,
        IAlumnoSuscripcionApiService alumnoSuscripcionService)
    {
        _alumnoApiService = alumnoApiService;
        _suscripcionService = suscripcionService;
        _alumnoSuscripcionService = alumnoSuscripcionService;
    }

    // GET /Alumnos?buscar=ana&suscripcion=Pase libre&altaDesde=2026-01-01&altaHasta=2026-12-31
    // Todos los filtros son opcionales y se pueden combinar.
    public async Task<IActionResult> Index(string? buscar, string? suscripcion, DateTime? altaDesde, DateTime? altaHasta)
    {
        var alumnos = await _alumnoApiService.ObtenerTodosAsync();

        // Texto libre: busca en nombre, apellido, DNI o email
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim().ToLower();
            alumnos = alumnos.Where(a =>
                $"{a.Nombre} {a.Apellido}".ToLower().Contains(texto) ||
                a.Dni.ToString().Contains(texto) ||
                (a.Email ?? "").ToLower().Contains(texto)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(suscripcion))
            alumnos = alumnos.Where(a => a.Suscripcion == suscripcion).ToList();

        if (altaDesde.HasValue)
            alumnos = alumnos.Where(a => a.FechaAlta.Date >= altaDesde.Value.Date).ToList();

        if (altaHasta.HasValue)
            alumnos = alumnos.Where(a => a.FechaAlta.Date <= altaHasta.Value.Date).ToList();

        // Se devuelven los filtros a la vista para que el formulario los siga mostrando
        ViewBag.Buscar = buscar;
        ViewBag.Suscripcion = suscripcion;
        ViewBag.AltaDesde = altaDesde?.ToString("yyyy-MM-dd");
        ViewBag.AltaHasta = altaHasta?.ToString("yyyy-MM-dd");
        ViewBag.Planes = await _suscripcionService.ObtenerTodasAsync();

        return View(alumnos.OrderBy(a => a.Apellido).ThenBy(a => a.Nombre).ToList());
    }

    // GET /Alumnos/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var detalle = await _alumnoApiService.ObtenerDetalleAsync(id);
        if (detalle == null) return NotFound();

        return View(detalle);
    }

    // GET /Alumnos/Create
    [SessionAuthorize("Administrador")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Planes = await _suscripcionService.ObtenerTodasAsync();
        return View();
    }

    // POST /Alumnos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SessionAuthorize("Administrador")]
    public async Task<IActionResult> Create(AlumnoCreateDto dto, int? suscripcionId)
    {
        var (ok, error, creado) = await _alumnoApiService.CrearAsync(dto);
        if (!ok || creado is null)
        {
            ViewBag.Error = error;
            ViewBag.Planes = await _suscripcionService.ObtenerTodasAsync();
            ViewBag.SuscripcionId = suscripcionId;
            return View(dto); // se devuelven los datos para no tener que cargarlos de nuevo
        }

        var errorSuscripcion = await CambiarSuscripcionAsync(creado.Id, suscripcionId);
        if (errorSuscripcion != null)
            TempData["Error"] = $"El cliente se registró, pero no se pudo asignar la suscripción: {errorSuscripcion}";
        else
            TempData["Mensaje"] = "Cliente registrado con éxito en el sistema";

        return RedirectToAction("Index");
    }

    // GET /Alumnos/Edit/5
    [SessionAuthorize("Administrador")]
    public async Task<IActionResult> Edit(int id)
    {
        var detalle = await _alumnoApiService.ObtenerDetalleAsync(id);
        if (detalle == null) return NotFound();

        await CargarSuscripcionesAsync(id);
        return View(detalle);
    }

    // POST /Alumnos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SessionAuthorize("Administrador")]
    public async Task<IActionResult> Edit(int id, AlumnoCreateDto dto, int? suscripcionId)
    {
        var (ok, error) = await _alumnoApiService.ActualizarAsync(id, dto);
        if (!ok)
        {
            ViewBag.Error = error;
            await CargarSuscripcionesAsync(id);
            return View(await _alumnoApiService.ObtenerDetalleAsync(id));
        }

        var errorSuscripcion = await CambiarSuscripcionAsync(id, suscripcionId);
        if (errorSuscripcion != null)
            TempData["Error"] = $"Los datos se guardaron, pero no se pudo cambiar la suscripción: {errorSuscripcion}";
        else
            TempData["Mensaje"] = "Los datos del cliente se han modificado con éxito en el sistema";

        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [SessionAuthorize("Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var eliminado = await _alumnoApiService.EliminarAsync(id);
        if (eliminado)
            TempData["Mensaje"] = "El cliente se ha dado de baja correctamente del sistema";
        else
            TempData["Error"] = "No se pudo dar de baja al cliente.";
        return RedirectToAction("Index");
    }

    // Carga los planes disponibles y el plan que el cliente tiene hoy (para preseleccionarlo)
    private async Task CargarSuscripcionesAsync(int alumnoId)
    {
        ViewBag.Planes = await _suscripcionService.ObtenerTodasAsync();
        var activa = (await _alumnoSuscripcionService.ObtenerHistorialAsync(alumnoId)).FirstOrDefault(s => s.Activa);
        ViewBag.SuscripcionId = activa?.SuscripcionId;
    }

    // Deja al cliente con el plan elegido en el formulario:
    //  - si eligió el mismo que ya tenía, no hace nada
    //  - si tenía otro, lo cancela y le asigna el nuevo
    //  - si eligió "Sin suscripción", solo cancela el que tenía
    // Devuelve null si salió todo bien, o el mensaje de error.
    private async Task<string?> CambiarSuscripcionAsync(int alumnoId, int? suscripcionId)
    {
        var activa = (await _alumnoSuscripcionService.ObtenerHistorialAsync(alumnoId)).FirstOrDefault(s => s.Activa);
        if (activa?.SuscripcionId == suscripcionId) return null;

        if (activa != null)
            await _alumnoSuscripcionService.CancelarAsync(alumnoId, activa.Id);

        if (!suscripcionId.HasValue) return null;

        var (ok, error) = await _alumnoSuscripcionService.AsignarAsync(alumnoId, suscripcionId.Value);
        return ok ? null : error;
    }
}
