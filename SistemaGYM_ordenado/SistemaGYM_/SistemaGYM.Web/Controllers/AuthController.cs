using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Logica.DTOs;
using SistemaGYM.Web.Services;

namespace SistemaGYM.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthApiService _authService;
    private readonly IAlumnoApiService _alumnoService;
    private readonly ISuscripcionApiService _suscripcionService;
    private readonly IAlumnoSuscripcionApiService _alumnoSuscripcionService;

    public AuthController(
        IAuthApiService authService,
        IAlumnoApiService alumnoService,
        ISuscripcionApiService suscripcionService,
        IAlumnoSuscripcionApiService alumnoSuscripcionService)
    {
        _authService = authService;
        _alumnoService = alumnoService;
        _suscripcionService = suscripcionService;
        _alumnoSuscripcionService = alumnoSuscripcionService;
    }

    // GET /Auth/Login
    [HttpGet]
    public IActionResult Login()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Rol")))
            return RedirectSegunRol(HttpContext.Session.GetString("Rol")!);

        return View();
    }

    // POST /Auth/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string contrasenia)
    {
        var resultado = await _authService.LoginAsync(email, contrasenia);
        if (resultado is null)
        {
            ViewBag.Error = "Email o contraseña incorrectos";
            return View();
        }

        HttpContext.Session.SetString("UserId", resultado.Id.ToString());
        HttpContext.Session.SetString("Nombre", resultado.Nombre);
        HttpContext.Session.SetString("Apellido", resultado.Apellido);
        HttpContext.Session.SetString("Rol", resultado.Rol);

        return RedirectSegunRol(resultado.Rol);
    }

    private IActionResult RedirectSegunRol(string rol) => rol switch
    {
        "Administrador" => RedirectToAction("Index", "Admin"),
        "Alumno" => RedirectToAction("Index", "Cliente"),
        "Profesor" => RedirectToAction("Index", "ProfesorPanel"),
        _ => RedirectToAction("Login")
    };

    // GET /Auth/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public IActionResult AccesoDenegado() => View();

    // GET /Auth/Registro
    [HttpGet]
    public async Task<IActionResult> Registro()
    {
        ViewBag.Suscripciones = await _suscripcionService.ObtenerTodasAsync();
        return View();
    }

    // POST /Auth/Registro
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(AlumnoCreateDto dto, string confirmarContrasenia, int? suscripcionId)
    {
        ViewBag.Suscripciones = await _suscripcionService.ObtenerTodasAsync();

        if (dto.Contrasenia != confirmarContrasenia)
        {
            ViewBag.Error = "Las contraseñas no coinciden";
            return View();
        }

        var dtoActivo = dto with { EstaActivo = true };
        var (ok, error, creado) = await _alumnoService.CrearAsync(dtoActivo);
        if (!ok || creado is null)
        {
            ViewBag.Error = error;
            return View();
        }

        // La API devuelve el cliente creado, así que usamos su Id para asignarle el plan
        if (suscripcionId.HasValue)
            await _alumnoSuscripcionService.AsignarAsync(creado.Id, suscripcionId.Value);

        ViewBag.Exito = true;
        return View();
    }

    // GET /Auth/RecuperarContrasenia
    [HttpGet]
    public IActionResult RecuperarContrasenia() => View();

    // POST /Auth/RecuperarContrasenia
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecuperarContrasenia(string email, int dni, string nuevaContrasenia, string confirmarContrasenia)
    {
        if (nuevaContrasenia != confirmarContrasenia)
        {
            ViewBag.Error = "Las contraseñas no coinciden";
            return View();
        }

        var (ok, error) = await _authService.ResetearContraseniaAsync(email, dni, nuevaContrasenia);
        if (!ok)
        {
            ViewBag.Error = error;
            return View();
        }

        ViewBag.Exito = true;
        return View();
    }
}
