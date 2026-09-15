using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SistemaGYM.Web.Filters;

// Reemplazo simple de [Authorize] usando la sesión de servidor.
// Uso: [SessionAuthorize("Administrador")] o [SessionAuthorize("Administrador", "Alumno")]
public class SessionAuthorizeAttribute : ActionFilterAttribute
{
    private readonly string[] _rolesPermitidos;

    public SessionAuthorizeAttribute(params string[] rolesPermitidos)
    {
        _rolesPermitidos = rolesPermitidos;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var rol = context.HttpContext.Session.GetString("Rol");

        if (string.IsNullOrEmpty(rol))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        if (_rolesPermitidos.Length > 0 && !_rolesPermitidos.Contains(rol))
        {
            context.Result = new RedirectToActionResult("AccesoDenegado", "Auth", null);
            return;
        }

        base.OnActionExecuting(context);
    }
}
