using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;

public static class AlimentacionEndpoints
{
    public static void MapAlimentacionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/alimentacion");

        group.MapGet("/", async (IAlimentacionLogica logica) =>
        {
            var lista = await logica.ObtenerTodosAsync();

            return Results.Ok(new { status = 200, message = "Planes de alimentación obtenidos correctamente", data = lista });
        })
        .WithName("GetAllAlimentacion")
        .WithSummary("Obtiene todos los planes de alimentación")
        .WithDescription("Retorna la lista completa de planes de alimentación registrados en el sistema.")
        .WithTags("Alimentacion")
        .Produces<IEnumerable<AlimentacionDto>>(200)
        .Produces(500);

        group.MapGet("/{id:int}", async (int id, IAlimentacionLogica logica) =>
        {
            var item = await logica.ObtenerPorIdAsync(id);
            if (item is null)
                return Results.Json(new { status = 404, message = "Plan de alimentación no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Plan de alimentación encontrado", data = item });
        })
        .WithName("GetAlimentacionById")
        .WithSummary("Obtiene un plan de alimentación por ID")
        .WithDescription("Retorna los datos de un plan de alimentación específico según su ID.")
        .WithTags("Alimentacion")
        .Produces<AlimentacionDto>(200)
        .Produces(404)
        .Produces(500);

        group.MapPost("/", async (AlimentacionCreateDto dto, IAlimentacionLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Json(new { status = 201, message = "Plan de alimentación creado correctamente", data = creado }, statusCode: 201);
        })
        .WithName("CreateAlimentacion")
        .WithSummary("Crea un nuevo plan de alimentación")
        .WithDescription("Da de alta un nuevo plan de alimentación asociado a un profesor.")
        .WithTags("Alimentacion")
        .Produces<AlimentacionDto>(201)
        .Produces(400)
        .Produces(500);

        group.MapPut("/{id:int}", async (int id, AlimentacionCreateDto dto, IAlimentacionLogica logica) =>
        {
            var modificado = await logica.ActualizarAsync(id, dto);
            if (!modificado)
                return Results.Json(new { status = 404, message = "Plan de alimentación no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Plan de alimentación actualizado correctamente" });
        })
        .WithName("UpdateAlimentacion")
        .WithSummary("Actualiza un plan de alimentación existente")
        .WithDescription("Modifica los datos de un plan de alimentación ya registrado.")
        .WithTags("Alimentacion")
        .Produces(200)
        .Produces(404)
        .Produces(400)
        .Produces(500);

        group.MapDelete("/{id:int}", async (int id, IAlimentacionLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            if (!eliminado)
                return Results.Json(new { status = 404, message = "Plan de alimentación no encontrado" }, statusCode: 404);

            return Results.NoContent();
        })
        .WithName("DeleteAlimentacion")
        .WithSummary("Elimina un plan de alimentación")
        .WithDescription("Elimina un plan de alimentación del sistema.")
        .WithTags("Alimentacion")
        .Produces(204)
        .Produces(404)
        .Produces(500);
    }
}
