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
            return Results.Ok(new { status = 200, message = "Alumnos obtenidos correctamente", data = alumnos });
        })
        .WithName("GetAllAlumnos")
        .WithSummary("Obtiene todos los alumnos")
        .WithDescription("Retorna una lista de todos los alumnos registrados en el sistema con datos abreviados (DTO).")
        .WithTags("Alumno")
        .Produces<IEnumerable<AlumnoDto>>(200)
        .Produces(500);

        // GET /api/alumnos/{id}
        group.MapGet("/{id:int}", async (int id, IAlumnoLogica logica) =>
        {
            var alumno = await logica.ObtenerPorIdAsync(id);
            if (alumno == null)
                return Results.Json(new { status = 404, message = "Alumno no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Alumno encontrado", data = alumno });
        })
        .WithName("GetAlumnoById")
        .WithSummary("Obtiene un alumno por ID")
        .WithDescription("Retorna los datos abreviados de un alumno específico según su ID.")
        .WithTags("Alumno")
        .Produces<AlumnoDto>(200)
        .Produces(404)
        .Produces(500);

        // GET /api/alumnos/{id}/detalle
        group.MapGet("/{id:int}/detalle", async (int id, IAlumnoLogica logica) =>
        {
            var detalle = await logica.ObtenerDetallePorIdAsync(id);
            if (detalle == null)
                return Results.Json(new { status = 404, message = "Alumno no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Alumno detalle encontrado", data = detalle });
        })
        .WithName("GetAlumnoDetalle")
        .WithSummary("Obtiene el detalle completo de un alumno")
        .WithDescription("Retorna todos los datos del alumno incluyendo dirección, teléfono, email y su suscripción activa.")
        .WithTags("Alumno")
        .Produces<AlumnoDetalleDto>(200)
        .Produces(404)
        .Produces(500);

        // POST /api/alumnos
        group.MapPost("/", async (AlumnoCreateDto dto, IAlumnoLogica logica) =>
        {
            var creado = await logica.CrearAsync(dto);
            return Results.Json(new { status = 201, message = "Alumno creado correctamente", data = creado }, statusCode: 201);
        })
        .WithName("CreateAlumno")
        .WithSummary("Crea un nuevo alumno")
        .WithDescription("Da de alta un nuevo alumno en el sistema con sus datos personales.")
        .WithTags("Alumno")
        .Produces<AlumnoDto>(201)
        .Produces(400)
        .Produces(500);

        // PUT /api/alumnos/{id}
        group.MapPut("/{id:int}", async (int id, AlumnoCreateDto dto, IAlumnoLogica logica) =>
        {
            var actualizado = await logica.ActualizarAsync(id, dto);
            if (!actualizado)
                return Results.Json(new { status = 404, message = "Alumno no encontrado" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Alumno actualizado correctamente" });
        })
        .WithName("UpdateAlumno")
        .WithSummary("Actualiza los datos de un alumno")
        .WithDescription("Modifica los datos personales de un alumno existente.")
        .WithTags("Alumno")
        .Produces(200)
        .Produces(404)
        .Produces(400)
        .Produces(500);

        // DELETE /api/alumnos/{id}
        group.MapDelete("/{id:int}", async (int id, IAlumnoLogica logica) =>
        {
            var eliminado = await logica.EliminarAsync(id);
            if (!eliminado)
                return Results.Json(new { status = 404, message = "Alumno no encontrado" }, statusCode: 404);

            return Results.NoContent();
        })
        .WithName("DeleteAlumno")
        .WithSummary("Elimina un alumno")
        .WithDescription("Elimina un alumno del sistema.")
        .WithTags("Alumno")
        .Produces(204)
        .Produces(404)
        .Produces(500);
    }
}
