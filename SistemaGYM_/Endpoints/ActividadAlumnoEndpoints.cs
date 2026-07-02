using SistemaGYM.Logica;
using SistemaGYM.Logica.DTOs;

namespace SistemaGYM.Endpoints;


public static class ActividadAlumnoEndpoints
{
    public static void MapActividadAlumnoEndpoints(this IEndpointRouteBuilder app)
    {
        var alumnosGroup = app.MapGroup("/api/alumnos").WithTags("ActividadAlumno");
        var actividadesGroup = app.MapGroup("/api/actividades").WithTags("ActividadAlumno");

        alumnosGroup.MapPost("/{id:int}/actividades/{actividadId:int}",
            async (int id, int actividadId, IActividadAlumnoLogica logica) =>
        {
            var (resultado, data) = await logica.InscribirAsync(id, actividadId);

            return resultado switch
            {
                ResultadoInscripcion.AlumnoNoEncontrado =>
                    Results.Json(new { status = 404, message = "Alumno no encontrado" }, statusCode: 404),
                ResultadoInscripcion.ActividadNoEncontrada =>
                    Results.Json(new { status = 404, message = "Actividad no encontrada" }, statusCode: 404),
                ResultadoInscripcion.YaInscripto =>
                    Results.Json(new { status = 409, message = "El alumno ya está inscripto en esta actividad" }, statusCode: 409),
                _ =>
                    Results.Json(new { status = 201, message = "Inscripción realizada correctamente", data }, statusCode: 201)
            };
        })
        .WithName("InscribirAlumnoEnActividad")
        .WithSummary("Inscribe a un alumno en una actividad")
        .WithDescription("Da de alta la inscripción del alumno a la actividad indicada. Si ya existía una inscripción inactiva, la reactiva.")
        .Produces<InscripcionDto>(201)
        .Produces(404)
        .Produces(409)
        .Produces(500);

        alumnosGroup.MapDelete("/{id:int}/actividades/{actividadId:int}",
            async (int id, int actividadId, IActividadAlumnoLogica logica) =>
        {
            var resultado = await logica.DarDeBajaAsync(id, actividadId);

            if (resultado == ResultadoInscripcion.InscripcionNoEncontrada)
                return Results.Json(new { status = 404, message = "No se encontró una inscripción activa para ese alumno en esa actividad" }, statusCode: 404);

            return Results.NoContent();
        })
        .WithName("DarDeBajaInscripcion")
        .WithSummary("Da de baja la inscripción de un alumno en una actividad")
        .WithDescription("Marca como inactiva la inscripción del alumno a la actividad indicada.")
        .Produces(204)
        .Produces(404)
        .Produces(500);

        actividadesGroup.MapGet("/{id:int}/alumnos",
            async (int id, IActividadAlumnoLogica logica) =>
        {
            var (resultado, data) = await logica.ObtenerAlumnosPorActividadAsync(id);

            if (resultado == ResultadoInscripcion.ActividadNoEncontrada)
                return Results.Json(new { status = 404, message = "Actividad no encontrada" }, statusCode: 404);

            return Results.Ok(new { status = 200, message = "Alumnos inscriptos obtenidos correctamente", data });
        })
        .WithName("GetAlumnosPorActividad")
        .WithSummary("Obtiene los alumnos inscriptos en una actividad")
        .WithDescription("Retorna la lista de alumnos actualmente inscriptos y activos en la actividad indicada.")
        .Produces<IEnumerable<AlumnoInscriptoDto>>(200)
        .Produces(404)
        .Produces(500);
    }
}