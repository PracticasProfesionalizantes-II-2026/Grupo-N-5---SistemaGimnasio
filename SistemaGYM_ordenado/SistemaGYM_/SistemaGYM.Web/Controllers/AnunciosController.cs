using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

// Admin: publica y elimina cualquier anuncio.
// Profesor: publica anuncios a su nombre y elimina solo los suyos.
// Alumno: solo los lee en Notificaciones.
[SessionAuthorize("Administrador", "Alumno", "Profesor")]
public class AnunciosController : Controller
{
    private readonly IAnuncioApiService _anuncioService;
    private readonly IProfesorApiService _profesorService;

    public AnunciosController(IAnuncioApiService anuncioService, IProfesorApiService profesorService)
    {
        _anuncioService = anuncioService;
        _profesorService = profesorService;
    }

    private string? Rol => HttpContext.Session.GetString("Rol");
    private bool EsAdmin => Rol == "Administrador";
    private bool EsProfesor => Rol == "Profesor";
    private int UsuarioId => int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

    // GET /Anuncios -> gestión (Admin: todos, Profesor: los suyos)
    public async Task<IActionResult> Index()
    {
        if (!EsAdmin && !EsProfesor) return RedirectToAction("Notificaciones");

        var anuncios = await _anuncioService.ObtenerTodosAsync();
        if (EsProfesor)
            anuncios = anuncios.Where(a => a.ProfesorId == UsuarioId).ToList();

        return View(anuncios.OrderByDescending(a => a.FechaPublicacion).ToList());
    }

    // GET /Anuncios/Notificaciones -> vista Cliente
    public async Task<IActionResult> Notificaciones()
    {
        var anuncios = (await _anuncioService.ObtenerTodosAsync())
            .OrderByDescending(a => a.FechaPublicacion)
            .ToList();
        return View(anuncios);
    }

    public async Task<IActionResult> Create()
    {
        if (!EsAdmin && !EsProfesor) return RedirectToAction("AccesoDenegado", "Auth");
        ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AnuncioCreateDto dto)
    {
        if (!EsAdmin && !EsProfesor) return RedirectToAction("AccesoDenegado", "Auth");

        // El profesor siempre publica a su nombre
        if (EsProfesor) dto = dto with { ProfesorId = UsuarioId };

        var (ok, error) = await _anuncioService.CrearAsync(dto);
        if (!ok)
        {
            ViewBag.Error = error;
            ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
            return View();
        }

        TempData["Mensaje"] = "Anuncio publicado con éxito en el sistema";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var anuncio = (await _anuncioService.ObtenerTodosAsync()).FirstOrDefault(a => a.Id == id);
        if (anuncio == null) return NotFound();

        var puedeEliminar = EsAdmin || (EsProfesor && anuncio.ProfesorId == UsuarioId);
        if (!puedeEliminar) return RedirectToAction("AccesoDenegado", "Auth");

        await _anuncioService.EliminarAsync(id);
        TempData["Mensaje"] = "El anuncio se ha eliminado con éxito del sistema";
        return RedirectToAction("Index");
    }
}
