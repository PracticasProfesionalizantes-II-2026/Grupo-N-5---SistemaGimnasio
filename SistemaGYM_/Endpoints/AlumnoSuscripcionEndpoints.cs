using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;

public static class AlumnoSuscripcionEndpoints
{
    public static void MapAlumnoSuscripcionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/alumnos/{id:int}/suscripciones");

        // GET /api/alumnos/{id}/suscripciones
        group.MapGet("/", async (int id, IAlumnoSuscripcionLogica logica) =>
        {
            var (resultado, data) = await logica.ObtenerHistorialAsync(id);

            if (resultado == ResultadoSuscripcion.AlumnoNoEncontrado)
                return Results.Json(new { status = 404, message = "Alumno no encontrado" }, statusCode: 404);


            return Results.Ok(new { status = 200, message = "Historial de suscripciones obtenido correctamente", data });
        })
        .WithName("GetHistorialSuscripciones")
        .WithSummary("Obtiene el historial de suscripciones de un alumno")
        .WithDescription("Retorna todas las suscripciones (activas e inactivas) asociadas a un alumno, ordenadas por fecha de inicio descendente.")
        .WithTags("AlumnoSuscripcion")
        .Produces(200)
        .Produces(404)
        .Produces(500);

        // POST /api/alumnos/{id}/suscripciones
        group.MapPost("/", async (int id, AsignarSuscripcionDto dto, IAlumnoSuscripcionLogica logica) =>
        {
            var (resultado, data) = await logica.AsignarAsync(id, dto);

            return resultado switch
            {
                ResultadoSuscripcion.AlumnoNoEncontrado =>
                    Results.Json(new { status = 404, message = "Alumno no encontrado" }, statusCode: 404),
                ResultadoSuscripcion.PlanNoEncontrado =>
                    Results.Json(new { status = 404, message = "El plan de suscripción indicado no existe" }, statusCode: 404),
                ResultadoSuscripcion.YaTieneSuscripcionActiva =>
                    Results.Json(new { status = 409, message = "El alumno ya tiene una suscripción activa" }, statusCode: 409),
                _ =>
                    Results.Json(new { status = 201, message = "Suscripción asignada correctamente", data }, statusCode: 201)
            };
        })
        .WithName("AsignarSuscripcion")
        .WithSummary("Asigna un plan de suscripción a un alumno")
        .WithDescription("Da de alta una nueva suscripción activa para el alumno, calculando la fecha de fin según la duración del plan. Falla si el alumno ya tiene una suscripción activa.")
        .WithTags("AlumnoSuscripcion")
        .Produces(201)
        .Produces(404)
        .Produces(409)
        .Produces(422)
        .Produces(500);

        // DELETE /api/alumnos/{id}/suscripciones/{alumnoSuscripcionId}
        group.MapDelete("/{alumnoSuscripcionId:int}", async (int id, int alumnoSuscripcionId, IAlumnoSuscripcionLogica logica) =>
        {
            var resultado = await logica.CancelarAsync(id, alumnoSuscripcionId);

            if (resultado == ResultadoSuscripcion.SuscripcionNoEncontrada)
                return Results.Json(new { status = 404, message = "No se encontró una suscripción activa con ese ID para este alumno" }, statusCode: 404);

            return Results.NoContent();
        })
        .WithName("CancelarSuscripcion")
        .WithSummary("Cancela la suscripción activa de un alumno")
        .WithDescription("Marca la suscripción como inactiva y cierra la fecha de fin en el momento de la cancelación.")
        .WithTags("AlumnoSuscripcion")
        .Produces(204)
        .Produces(404)
        .Produces(500);
    }
}