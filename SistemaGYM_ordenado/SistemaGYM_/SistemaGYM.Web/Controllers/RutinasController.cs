using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

// Admin: gestiona todas las rutinas.
// Profesor: crea rutinas a su nombre (mismo formulario que el admin, o desde una actividad)
//           y modifica/elimina solo las suyas.
// Alumno: ve las rutinas asignadas a él y las generales.
// Cada rutina es un único registro asignado a 0, 1 o varios alumnos (sin alumnos = general).
[SessionAuthorize("Administrador", "Alumno", "Profesor")]
public class RutinasController : Controller
{
    private readonly IRutinaApiService _rutinaService;
    private readonly IAlumnoApiService _alumnoService;
    private readonly IProfesorApiService _profesorService;
    private readonly IActividadApiService _actividadService;

    public RutinasController(IRutinaApiService rutinaService, IAlumnoApiService alumnoService, IProfesorApiService profesorService, IActividadApiService actividadService)
    {
        _rutinaService = rutinaService;
        _alumnoService = alumnoService;
        _profesorService = profesorService;
        _actividadService = actividadService;
    }

    private string? Rol => HttpContext.Session.GetString("Rol");
    private bool EsAdmin => Rol == "Administrador";
    private bool EsProfesor => Rol == "Profesor";
    private int UsuarioId => int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

    // El admin puede tocar cualquier rutina; el profesor solo las suyas
    private bool PuedeGestionar(RutinaDto rutina) => EsAdmin || (EsProfesor && rutina.ProfesorId == UsuarioId);

    // A dónde volver después de crear, modificar o eliminar
    private IActionResult VolverAlListado() =>
        EsProfesor ? RedirectToAction("Rutinas", "ProfesorPanel") : RedirectToAction("Index");

    private async Task<RutinaDto?> BuscarAsync(int id) =>
        (await _rutinaService.ObtenerTodasAsync()).FirstOrDefault(r => r.Id == id);

    // GET /Rutinas -> gestión completa (solo Admin)
    public async Task<IActionResult> Index()
    {
        if (EsProfesor) return RedirectToAction("Rutinas", "ProfesorPanel");
        if (!EsAdmin) return RedirectToAction("MisRutinas");

        ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
        return View(await _rutinaService.ObtenerTodasAsync());
    }

    // GET /Rutinas/MisRutinas -> vista del Cliente
    public async Task<IActionResult> MisRutinas()
    {
        if (Rol != "Alumno") return RedirectToAction("Index");
        return View(await _rutinaService.ObtenerDeAlumnoAsync(UsuarioId));
    }

    // GET /Rutinas/Details/5 -> descripción completa (se abre al hacer clic en la tarjeta)
    public async Task<IActionResult> Details(int id)
    {
        var rutina = await BuscarAsync(id);
        if (rutina == null) return NotFound();

        // El alumno solo puede ver sus rutinas y las generales
        var esDelAlumno = rutina.Alumnos.Count == 0 || rutina.Alumnos.Any(a => a.AlumnoId == UsuarioId);
        if (Rol == "Alumno" && !esDelAlumno) return RedirectToAction("AccesoDenegado", "Auth");

        var profesor = (await _profesorService.ObtenerTodosAsync()).FirstOrDefault(p => p.Id == rutina.ProfesorId);
        ViewBag.NombreProfesor = profesor == null ? "Profesor dado de baja" : $"{profesor.Nombre} {profesor.Apellido}";
        ViewBag.PuedeGestionar = PuedeGestionar(rutina);
        return View(rutina);
    }

    // Alumnos, profesores y actividades para los combos.
    // El profesor solo puede elegir sus propias actividades.
    private async Task CargarCombosAsync()
    {
        ViewBag.Alumnos = await _alumnoService.ObtenerTodosAsync();
        ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
        var actividades = await _actividadService.ObtenerTodasAsync();
        if (EsProfesor) actividades = actividades.Where(a => a.ProfesorId == UsuarioId).ToList();
        ViewBag.Actividades = actividades;

        // Alumnos inscriptos en cada actividad: al elegir una actividad, la vista muestra solo esos alumnos
        var inscriptosPorActividad = new Dictionary<int, List<int>>();
        foreach (var a in actividades)
        {
            var inscriptos = await _actividadService.ObtenerAlumnosInscriptosAsync(a.ActividadId);
            inscriptosPorActividad[a.ActividadId] = inscriptos.Select(i => i.AlumnoId).ToList();
        }
        ViewBag.InscriptosPorActividad = inscriptosPorActividad;
    }

    public async Task<IActionResult> Create()
    {
        if (!EsAdmin && !EsProfesor) return RedirectToAction("AccesoDenegado", "Auth");
        await CargarCombosAsync();
        return View();
    }

    // Se guarda una sola rutina para todos los alumnos elegidos (sin alumnos = rutina general)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RutinaCreateDto dto)
    {
        if (!EsAdmin && !EsProfesor) return RedirectToAction("AccesoDenegado", "Auth");

        // El profesor siempre crea la rutina a su nombre
        if (EsProfesor) dto = dto with { ProfesorId = UsuarioId };

        var (ok, error) = await _rutinaService.CrearAsync(dto);
        if (!ok)
        {
            ViewBag.Error = error;
            await CargarCombosAsync();
            ViewBag.AlumnosSeleccionados = dto.AlumnoIds;
            return View();
        }

        TempData["Mensaje"] = "Rutina registrada con éxito en el sistema";
        return VolverAlListado();
    }

    public async Task<IActionResult> Edit(int id)
    {
        var rutina = await BuscarAsync(id);
        if (rutina == null) return NotFound();
        if (!PuedeGestionar(rutina)) return RedirectToAction("AccesoDenegado", "Auth");

        await CargarCombosAsync();
        ViewBag.AlumnosSeleccionados = rutina.Alumnos.Select(a => a.AlumnoId).ToList();
        return View(rutina);
    }

    // Como es una sola rutina, el cambio lo ven todos los alumnos asignados
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RutinaCreateDto dto)
    {
        var rutina = await BuscarAsync(id);
        if (rutina == null) return NotFound();
        if (!PuedeGestionar(rutina)) return RedirectToAction("AccesoDenegado", "Auth");

        // Un profesor no puede pasarle su rutina a otro profesor
        if (EsProfesor) dto = dto with { ProfesorId = UsuarioId };

        var (ok, error) = await _rutinaService.ActualizarAsync(id, dto);
        if (!ok)
        {
            ViewBag.Error = error;
            await CargarCombosAsync();
            ViewBag.AlumnosSeleccionados = dto.AlumnoIds;
            return View(rutina);
        }

        TempData["Mensaje"] = "Los datos de la rutina se han modificado con éxito en el sistema";
        return VolverAlListado();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var rutina = await BuscarAsync(id);
        if (rutina == null) return NotFound();
        if (!PuedeGestionar(rutina)) return RedirectToAction("AccesoDenegado", "Auth");

        var (ok, error) = await _rutinaService.EliminarAsync(id);
        if (ok)
            TempData["Mensaje"] = "La rutina se ha eliminado con éxito del sistema";
        else
            TempData["Error"] = error;
        return VolverAlListado();
    }
}
