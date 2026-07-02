using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;

public static class RutinaEndpoints
{
    public static void MapRutinaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rutinas");

        group.MapGet("/", async (IRutinaLogica logica) =>
        {
            var lista = await logica.ObtenerTodosAsync();

            return Results.Ok(new { status = 200, message = "Rutinas obtenidas correctamente", data = lista });
        })
        .WithName("GetAllRutinas")
        .WithSummary("Obtiene todas las rutinas")
        .WithDescription("Retorna la lista completa de rutinas registradas en el sistema.")
        .WithTags("Rutina")
        .Produces<IEnumerable<RutinaDto>>(200)
        .Produces(500);

        group.MapGet("/{id:int}", async (int id, IRutinaLogica logica) =>
        {
            var item = await logica.ObtenerPorIdAsync(id);
            if (item is null)
                return Results.Json(new { status = 404, message = "Rutina no encontrada" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Rutina encontrada", data = item });
        })
        .WithName("GetRutinaById")
        .WithSummary("Obtiene una rutina por ID")
        .WithDescription("Retorna los datos de una rutina específica según su ID.")
        .WithTags("Rutina")
        .Produces<RutinaDto>(200)
        .Produces(404)
        .Produces(500);

        group.MapPost("/", async (RutinaCreateDto dto, IRutinaLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Json(new { status = 201, message = "Rutina creada correctamente", data = creado }, statusCode: 201);
        })
        .WithName("CreateRutina")
        .WithSummary("Crea una nueva rutina")
        .WithDescription("Da de alta una nueva rutina asignada por un profesor a un alumno.")
        .WithTags("Rutina")
        .Produces<RutinaDto>(201)
        .Produces(400)
        .Produces(500);

        group.MapPut("/{id:int}", async (int id, RutinaCreateDto dto, IRutinaLogica logica) =>
        {
            var modificado = await logica.ActualizarAsync(id, dto);
            if (!modificado)
                return Results.Json(new { status = 404, message = "Rutina no encontrada" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Rutina actualizada correctamente" });
        })
        .WithName("UpdateRutina")
        .WithSummary("Actualiza una rutina existente")
        .WithDescription("Modifica los datos de una rutina ya registrada.")
        .WithTags("Rutina")
        .Produces(200)
        .Produces(404)
        .Produces(400)
        .Produces(500);

        group.MapDelete("/{id:int}", async (int id, IRutinaLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            if (!eliminado)
                return Results.Json(new { status = 404, message = "Rutina no encontrada" }, statusCode: 404);

            return Results.NoContent();
        })
        .WithName("DeleteRutina")
        .WithSummary("Elimina una rutina")
        .WithDescription("Elimina una rutina del sistema.")
        .WithTags("Rutina")
        .Produces(204)
        .Produces(404)
        .Produces(500);
    }
}
