using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;

public static class ActividadEndpoints
{
    public static void MapActividadEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/actividades");

        group.MapGet("/", async (IActividadLogica logica) =>
        {
            var lista = await logica.ObtenerTodosAsync();

            return Results.Ok(new { status = 200, message = "Actividades obtenidas correctamente", data = lista });
        })
        .WithName("GetAllActividades")
        .WithSummary("Obtiene todas las actividades")
        .WithDescription("Retorna la lista completa de actividades registradas en el sistema.")
        .WithTags("Actividad")
        .Produces<IEnumerable<ActividadDto>>(200)
        .Produces(500);

        group.MapGet("/{id:int}", async (int id, IActividadLogica logica) =>
        {
            var actividad = await logica.ObtenerPorIdAsync(id);
            if (actividad == null)
                return Results.Json(new { status = 404, message = "Actividad no encontrada" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Actividad encontrada", data = actividad });
        })
        .WithName("GetActividadById")
        .WithSummary("Obtiene una actividad por ID")
        .WithDescription("Retorna los datos de una actividad específica según su ID.")
        .WithTags("Actividad")
        .Produces<ActividadDto>(200)
        .Produces(404)
        .Produces(500);

        group.MapPost("/", async (ActividadCreateDto dto, IActividadLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Json(new { status = 201, message = "Actividad creada correctamente", data = creado }, statusCode: 201);
        })
        .WithName("CreateActividad")
        .WithSummary("Crea una nueva actividad")
        .WithDescription("Da de alta una nueva actividad del gimnasio, asociada a un profesor y a un día de la semana.")
        .WithTags("Actividad")
        .Produces<ActividadDto>(201)
        .Produces(500);

        group.MapPut("/{id:int}", async (int id, ActividadCreateDto dto, IActividadLogica logica) =>
        {
            var actualizado = await logica.ActualizarAsync(id, dto);
            if (!actualizado)
                return Results.Json(new { status = 404, message = "Actividad no encontrada" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Actividad actualizada correctamente" });
        })
        .WithName("UpdateActividad")
        .WithSummary("Actualiza una actividad existente")
        .WithDescription("Modifica los datos de una actividad ya registrada.")
        .WithTags("Actividad")
        .Produces(200)
        .Produces(404)
        .Produces(400)
        .Produces(500);

        group.MapDelete("/{id:int}", async (int id, IActividadLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            if (!eliminado)
                return Results.Json(new { status = 404, message = "Actividad no encontrada" }, statusCode: 404);

            return Results.NoContent();
        })
        .WithName("DeleteActividad")
        .WithSummary("Elimina una actividad")
        .WithDescription("Elimina una actividad del sistema.")
        .WithTags("Actividad")
        .Produces(204)
        .Produces(404)
        .Produces(500);
    }
}