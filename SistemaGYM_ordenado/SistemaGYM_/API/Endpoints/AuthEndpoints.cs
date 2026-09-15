using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async (LoginDto dto, IAuthLogica logica) =>
        {
            var (resultado, data) = await logica.LoginAsync(dto);

            if (resultado == ResultadoLogin.CredencialesInvalidas)
                return Results.Json(new { status = 401, message = "Email o contraseña incorrectos" }, statusCode: 401);

            return Results.Ok(new { status = 200, message = "Login exitoso", data });
        })
        .WithName("Login")
        .WithSummary("Inicia sesión")
        .WithDescription("Valida las credenciales de un alumno, profesor o administrador. Todavía no emite token JWT, solo confirma identidad.")
        .Produces<LoginResultDto>(200)
        .Produces(401);

        group.MapPost("/resetear-contrasenia", async (ResetPasswordDto dto, IAuthLogica logica) =>
        {
            var resultado = await logica.ResetearContraseniaAsync(dto);

            if (resultado == ResultadoReset.UsuarioNoEncontrado)
                return Results.Json(new { status = 404, message = "No se encontró un usuario con ese email y DNI" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Contraseña actualizada con éxito" });
        })
        .WithName("ResetearContrasenia")
        .WithSummary("Restablece la contraseña de un alumno o profesor")
        .WithDescription("Verifica identidad por email + DNI (reemplazo simplificado del código de verificación) y actualiza la contraseña.")
        .Produces(200)
        .Produces(404);
    }
}