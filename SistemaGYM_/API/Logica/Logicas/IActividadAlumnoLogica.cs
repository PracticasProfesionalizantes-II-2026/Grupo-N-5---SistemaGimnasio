using SistemaGYM.Logica.DTOs;
using SistemaGYM.Entidades;
using SistemaGYM.Repositorios;

namespace SistemaGYM.Logica;

public enum ResultadoInscripcion
{
    Ok,
    AlumnoNoEncontrado,
    ActividadNoEncontrada,
    YaInscripto,
    InscripcionNoEncontrada
}

public interface IActividadAlumnoLogica
{
    Task<(ResultadoInscripcion resultado, InscripcionDto? data)> InscribirAsync(int alumnoId, int actividadId);
    Task<ResultadoInscripcion> DarDeBajaAsync(int alumnoId, int actividadId);
    Task<(ResultadoInscripcion resultado, IEnumerable<AlumnoInscriptoDto>? data)> ObtenerAlumnosPorActividadAsync(int actividadId);
}

public class ActividadAlumnoLogica : IActividadAlumnoLogica
{
    private readonly IActividadAlumnoRepository _repo;
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly IActividadRepository _actividadRepo;

    public ActividadAlumnoLogica(
        IActividadAlumnoRepository repo,
        IAlumnoRepository alumnoRepo,
        IActividadRepository actividadRepo)
    {
        _repo = repo;
        _alumnoRepo = alumnoRepo;
        _actividadRepo = actividadRepo;
    }

    public async Task<(ResultadoInscripcion, InscripcionDto?)> InscribirAsync(int alumnoId, int actividadId)
    {
        if (await _alumnoRepo.ObtenerPorIdAsync(alumnoId) is null)
            return (ResultadoInscripcion.AlumnoNoEncontrado, null);

        if (await _actividadRepo.ObtenerPorIdAsync(actividadId) is null)
            return (ResultadoInscripcion.ActividadNoEncontrada, null);

        var existente = await _repo.ObtenerAsync(alumnoId, actividadId);

        if (existente is { Activa: true })
            return (ResultadoInscripcion.YaInscripto, null);

        if (existente is { Activa: false })
        {
            existente.Activa = true;
            existente.FechaInscripcion = DateTime.Now;
            await _repo.ActualizarAsync(existente);

            return (ResultadoInscripcion.Ok, new InscripcionDto(
                existente.Id, existente.AlumnoId, existente.ActividadId,
                existente.FechaInscripcion, existente.Activa));
        }

        var nueva = new ActividadAlumno
        {
            AlumnoId = alumnoId,
            ActividadId = actividadId,
            FechaInscripcion = DateTime.Now,
            Activa = true
        };
        await _repo.AgregarAsync(nueva);

        return (ResultadoInscripcion.Ok, new InscripcionDto(
            nueva.Id, nueva.AlumnoId, nueva.ActividadId, nueva.FechaInscripcion, nueva.Activa));
    }

    public async Task<ResultadoInscripcion> DarDeBajaAsync(int alumnoId, int actividadId)
    {
        var inscripcion = await _repo.ObtenerAsync(alumnoId, actividadId);
        if (inscripcion is null || !inscripcion.Activa)
            return ResultadoInscripcion.InscripcionNoEncontrada;

        inscripcion.Activa = false;
        await _repo.ActualizarAsync(inscripcion);
        return ResultadoInscripcion.Ok;
    }

    public async Task<(ResultadoInscripcion, IEnumerable<AlumnoInscriptoDto>?)> ObtenerAlumnosPorActividadAsync(int actividadId)
    {
        if (await _actividadRepo.ObtenerPorIdAsync(actividadId) is null)
            return (ResultadoInscripcion.ActividadNoEncontrada, null);

        var inscriptos = await _repo.ObtenerPorActividadAsync(actividadId);

        var dtos = inscriptos.Select(i => new AlumnoInscriptoDto(
            i.AlumnoId, i.Alumno.Nombre, i.Alumno.Apellido, i.FechaInscripcion, i.Activa));

        return (ResultadoInscripcion.Ok, dtos);
    }
}