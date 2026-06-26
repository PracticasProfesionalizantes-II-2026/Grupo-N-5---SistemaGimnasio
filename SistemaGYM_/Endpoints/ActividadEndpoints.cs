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
            if (!lista.Any())
                return Results.Json(new { status = 204, message = "No se encontraron actividades" }, statusCode: 204);

            return Results.Ok(new { status = 200, message = "Actividades obtenidas correctamente", data = lista });
        });

       
        group.MapGet("/{id:int}", async (int id, IActividadLogica logica) =>
        {
            var actividad = await logica.ObtenerPorIdAsync(id);
            if (actividad == null)
                return Results.Json(new { status = 404, message = "Actividad no encontrada" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Actividad encontrada", data = actividad });
        });

        
        group.MapPost("/", async (ActividadCreateDto dto, IActividadLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Json(new { status = 201, message = "Actividad creada correctamente", data = creado }, statusCode: 201);
        });

        group.MapPut("/{id:int}", async (int id, ActividadCreateDto dto, IActividadLogica logica) =>
        {
            var actualizado = await logica.ActualizarAsync(id, dto);
            if (!actualizado)
                return Results.Json(new { status = 404, message = "Actividad no encontrada" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Actividad actualizada correctamente" });
        });

      
        group.MapDelete("/{id:int}", async (int id, IActividadLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            if (!eliminado)
                return Results.Json(new { status = 404, message = "Actividad no encontrada" }, statusCode: 404);

            return Results.Json(new { status = 204, message = "Actividad eliminada correctamente" }, statusCode: 204);
        });
    }
}