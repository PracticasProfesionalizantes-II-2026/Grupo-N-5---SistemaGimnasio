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
        .WithDescription("Valida las credenciales de un alumno o profesor. Todavía no emite token JWT, solo confirma identidad.")
        .Produces<LoginResultDto>(200)
        .Produces(401);
    }
}