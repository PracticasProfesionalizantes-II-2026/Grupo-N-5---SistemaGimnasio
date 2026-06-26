using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;

public static class AlumnoEndpoints
{
    public static void MapAlumnoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/alumnos");

        // GET /api/alumnos
        group.MapGet("/", async (IAlumnoLogica logica) =>
        {
            var alumnos = await logica.ObtenerTodosAsync();
            if (!alumnos.Any())
                return Results.Json(new { status = 204, message = "No se encontraron alumnos" }, statusCode: 204);

            return Results.Ok(new { status = 200, message = "Alumnos obtenidos correctamente", data = alumnos });
        });

        // GET /api/alumnos/{id}
        group.MapGet("/{id:int}", async (int id, IAlumnoLogica logica) =>
        {
            var alumno = await logica.ObtenerPorIdAsync(id);
            if (alumno == null)
                return Results.Json(new { status = 404, message = "Alumno no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Alumno encontrado", data = alumno });
        });

        // GET /api/alumnos/{id}/detalle
        group.MapGet("/{id:int}/detalle", async (int id, IAlumnoLogica logica) =>
        {
            var detalle = await logica.ObtenerDetallePorIdAsync(id);
            if (detalle == null)
                return Results.Json(new { status = 404, message = "Alumno no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Alumno detalle encontrado", data = detalle });
        });

        // POST /api/alumnos
        group.MapPost("/", async (AlumnoCreateDto dto, IAlumnoLogica logica) =>
        {
            // Nota: Podrías agregar validaciones acá para devolver el 422 de tu docu si hiciese falta.
            var creado = await logica.CrearAsync(dto);
            return Results.Json(new { status = 201, message = "Alumno creado correctamente", data = creado }, statusCode: 201);
        });

        // PUT /api/alumnos/{id}
        group.MapPut("/{id:int}", async (int id, AlumnoCreateDto dto, IAlumnoLogica logica) =>
        {
            var actualizado = await logica.ActualizarAsync(id, dto);
            if (!actualizado)
                return Results.Json(new { status = 404, message = "Alumno no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Alumno actualizado correctamente" });
        });

        // DELETE /api/alumnos/{id}
        group.MapDelete("/{id:int}", async (int id, IAlumnoLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            if (!eliminado)
                return Results.Json(new { status = 404, message = "Alumno no encontrado" }, statusCode: 404);

            return Results.Json(new { status = 204, message = "Alumno eliminado correctamente" }, statusCode: 204);
        });
    }
}