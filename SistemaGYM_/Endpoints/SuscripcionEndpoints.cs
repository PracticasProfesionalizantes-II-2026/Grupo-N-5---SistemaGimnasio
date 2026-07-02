using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;

public static class SuscripcionEndpoints
{
    public static void MapSuscripcionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/suscripciones");

        group.MapGet("/", async (ISuscripcionLogica logica) =>
        {
            var lista = await logica.ObtenerTodosAsync();

            return Results.Ok(new { status = 200, message = "Planes de suscripción obtenidos correctamente", data = lista });
        })
        .WithName("GetAllSuscripciones")
        .WithSummary("Obtiene todos los planes de suscripción")
        .WithDescription("Retorna la lista completa de planes de suscripción disponibles en el gimnasio.")
        .WithTags("Suscripcion")
        .Produces<IEnumerable<SuscripcionDto>>(200)
        .Produces(500);

        group.MapGet("/{id:int}", async (int id, ISuscripcionLogica logica) =>
        {
            var item = await logica.ObtenerPorIdAsync(id);
            if (item is null)
                return Results.Json(new { status = 404, message = "Plan de suscripción no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Plan de suscripción encontrado", data = item });
        })
        .WithName("GetSuscripcionById")
        .WithSummary("Obtiene un plan de suscripción por ID")
        .WithDescription("Retorna los datos de un plan de suscripción específico según su ID.")
        .WithTags("Suscripcion")
        .Produces<SuscripcionDto>(200)
        .Produces(404)
        .Produces(500);

        group.MapPost("/", async (SuscripcionCreateDto dto, ISuscripcionLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Json(new { status = 201, message = "Plan de suscripción creado correctamente", data = creado }, statusCode: 201);
        })
        .WithName("CreateSuscripcion")
        .WithSummary("Crea un nuevo plan de suscripción")
        .WithDescription("Da de alta un nuevo plan de suscripción con nombre, precio y duración en días.")
        .WithTags("Suscripcion")
        .Produces<SuscripcionDto>(201)
        .Produces(400)
        .Produces(500);

        group.MapPut("/{id:int}", async (int id, SuscripcionCreateDto dto, ISuscripcionLogica logica) =>
        {
            var modificado = await logica.ActualizarAsync(id, dto);
            if (!modificado)
                return Results.Json(new { status = 404, message = "Plan de suscripción no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Plan de suscripción actualizado correctamente" });
        })
        .WithName("UpdateSuscripcion")
        .WithSummary("Actualiza un plan de suscripción existente")
        .WithDescription("Modifica el nombre, precio o duración de un plan de suscripción ya registrado.")
        .WithTags("Suscripcion")
        .Produces(200)
        .Produces(404)
        .Produces(400)
        .Produces(500);

        group.MapDelete("/{id:int}", async (int id, ISuscripcionLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            if (!eliminado)
                return Results.Json(new { status = 404, message = "Plan de suscripción no encontrado" }, statusCode: 404);

            return Results.NoContent();
        })
        .WithName("DeleteSuscripcion")
        .WithSummary("Elimina un plan de suscripción")
        .WithDescription("Elimina un plan de suscripción del sistema.")
        .WithTags("Suscripcion")
        .Produces(204)
        .Produces(404)
        .Produces(500);
    }
}
