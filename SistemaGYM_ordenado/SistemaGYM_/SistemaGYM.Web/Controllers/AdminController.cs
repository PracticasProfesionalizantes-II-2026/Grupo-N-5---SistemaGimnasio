using Microsoft.AspNetCore.Mvc;
using SistemaGYM.Web.Filters;

namespace SistemaGYM.Web.Controllers;

[SessionAuthorize("Administrador")]
public class AdminController : Controller
{
    public IActionResult Index() => View();
}
