using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Filters;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Administrador", "Alumno")]
public class AnunciosController : Controller
{
    private readonly IAnuncioApiService _anuncioService;
    private readonly IProfesorApiService _profesorService;

    public AnunciosController(IAnuncioApiService anuncioService, IProfesorApiService profesorService)
    {
        _anuncioService = anuncioService;
        _profesorService = profesorService;
    }

    private bool EsAdmin => HttpContext.Session.GetString("Rol") == "Administrador";

    // GET /Anuncios -> gestión (Admin)
    public async Task<IActionResult> Index()
    {
        if (!EsAdmin) return RedirectToAction("Notificaciones");
        return View(await _anuncioService.ObtenerTodosAsync());
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
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");
        ViewBag.Profesores = await _profesorService.ObtenerTodosAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AnuncioCreateDto dto)
    {
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");

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
        if (!EsAdmin) return RedirectToAction("AccesoDenegado", "Auth");

        await _anuncioService.EliminarAsync(id);
        TempData["Mensaje"] = "El anuncio se ha eliminado con éxito del sistema";
        return RedirectToAction("Index");
    }
}
