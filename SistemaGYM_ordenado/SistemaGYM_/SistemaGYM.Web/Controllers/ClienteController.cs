using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Web.Filters;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Alumno", "Profesor")]
public class ClienteController : Controller
{
    public IActionResult Index() => View();
}
