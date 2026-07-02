using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;

public static class ProfesorEndpoints
{
    public static void MapProfesorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/profesores");

        group.MapGet("/", async (IProfesorLogica logica) =>
        {
            var lista = await logica.ObtenerTodosAsync();

            return Results.Ok(new { status = 200, message = "Profesores obtenidos correctamente", data = lista });
        })
        .WithName("GetAllProfesores")
        .WithSummary("Obtiene todos los profesores")
        .WithDescription("Retorna una lista de todos los profesores registrados en el sistema con datos abreviados (DTO).")
        .WithTags("Profesor")
        .Produces(200)
        .Produces(500);

        group.MapGet("/{id:int}", async (int id, IProfesorLogica logica) =>
        {
            var profesor = await logica.ObtenerPorIdAsync(id);
            if (profesor == null)
                return Results.Json(new { status = 404, message = "Profesor no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Profesor encontrado", data = profesor });
        })
        .WithName("GetProfesorById")
        .WithSummary("Obtiene un profesor por ID")
        .WithDescription("Retorna los datos abreviados de un profesor específico según su ID.")
        .WithTags("Profesor")
        .Produces<ProfesorDto>(200)
        .Produces(404)
        .Produces(500);

        group.MapGet("/{id:int}/detalle", async (int id, IProfesorLogica logica) =>
        {
            var detalle = await logica.ObtenerDetallePorIdAsync(id);
            if (detalle == null)
                return Results.Json(new { status = 404, message = "Profesor no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Profesor detalle encontrado", data = detalle });
        })
        .WithName("GetProfesorDetalle")
        .WithSummary("Obtiene el detalle completo de un profesor")
        .WithDescription("Retorna todos los datos del profesor incluyendo dirección, teléfono y actividades asignadas.")
        .WithTags("Profesor")
        .Produces<ProfesorDetalleDto>(200)
        .Produces(404)
        .Produces(500);

        group.MapPost("/", async (ProfesorCreateDto dto, IProfesorLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Json(new { status = 201, message = "Profesor creado correctamente", data = creado }, statusCode: 201);
        })
        .WithName("CreateProfesor")
        .WithSummary("Crea un nuevo profesor")
        .WithDescription("Da de alta un nuevo profesor en el sistema con todos sus datos personales.")
        .WithTags("Profesor")
        .Produces<ProfesorDto>(201)
        .Produces(400)
        .Produces(403)
        .Produces(500);

        group.MapPut("/{id:int}", async (int id, ProfesorCreateDto dto, IProfesorLogica logica) =>
        {
            var actualizado = await logica.ActualizarAsync(id, dto);
            if (!actualizado)
                return Results.Json(new { status = 404, message = "Profesor no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Profesor actualizado correctamente" });
        })
        .WithName("UpdateProfesor")
        .WithSummary("Actualiza los datos de un profesor")
        .WithDescription("Modifica los datos personales y profesionales de un profesor existente.")
        .WithTags("Profesor")
        .Produces(200)
        .Produces(404)
        .Produces(400)
        .Produces(401)
        .Produces(500);

        group.MapDelete("/{id:int}", async (int id, IProfesorLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            if (!eliminado)
                return Results.Json(new { status = 404, message = "Profesor no encontrado" }, statusCode: 404);

            return Results.NoContent();
        })
        .WithName("DeleteProfesor")
        .WithSummary("Elimina un profesor")
        .WithDescription("Elimina un profesor del sistema. No se puede eliminar si tiene actividades o rutinas asignadas.")
        .WithTags("Profesor")
        .Produces(204)
        .Produces(404)
        .Produces(403)
        .Produces(422)
        .Produces(500);
    }
}
