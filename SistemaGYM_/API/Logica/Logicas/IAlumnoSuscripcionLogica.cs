using SistemaGYM.Logica.DTOs;
using SistemaGYM.Entidades;
using SistemaGYM.Repositorios;

namespace SistemaGYM.Logica;

public enum ResultadoSuscripcion
{
    Ok,
    AlumnoNoEncontrado,
    PlanNoEncontrado,
    YaTieneSuscripcionActiva,
    SuscripcionNoEncontrada
}

public interface IAlumnoSuscripcionLogica
{
    Task<(ResultadoSuscripcion resultado, IEnumerable<AlumnoSuscripcionDto>? data)> ObtenerHistorialAsync(int alumnoId);
    Task<(ResultadoSuscripcion resultado, AlumnoSuscripcionDto? data)> AsignarAsync(int alumnoId, AsignarSuscripcionDto dto);
    Task<ResultadoSuscripcion> CancelarAsync(int alumnoId, int alumnoSuscripcionId);
}

public class AlumnoSuscripcionLogica : IAlumnoSuscripcionLogica
{
    private readonly IAlumnoSuscripcionRepository _repo;
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly ISuscripcionRepository _suscripcionRepo;

    public AlumnoSuscripcionLogica(
        IAlumnoSuscripcionRepository repo,
        IAlumnoRepository alumnoRepo,
        ISuscripcionRepository suscripcionRepo)
    {
        _repo = repo;
        _alumnoRepo = alumnoRepo;
        _suscripcionRepo = suscripcionRepo;
    }

    public async Task<(ResultadoSuscripcion, IEnumerable<AlumnoSuscripcionDto>?)> ObtenerHistorialAsync(int alumnoId)
    {
        if (await _alumnoRepo.ObtenerPorIdAsync(alumnoId) is null)
            return (ResultadoSuscripcion.AlumnoNoEncontrado, null);

        var historial = await _repo.ObtenerPorAlumnoAsync(alumnoId);

        var dtos = historial.Select(h => new AlumnoSuscripcionDto(
            h.Id, h.SuscripcionId, h.Suscripcion.Nombre, h.Suscripcion.Precio,
            h.FechaInicio, h.FechaFin, h.Activa));

        return (ResultadoSuscripcion.Ok, dtos);
    }

    public async Task<(ResultadoSuscripcion, AlumnoSuscripcionDto?)> AsignarAsync(int alumnoId, AsignarSuscripcionDto dto)
    {
        if (await _alumnoRepo.ObtenerPorIdAsync(alumnoId) is null)
            return (ResultadoSuscripcion.AlumnoNoEncontrado, null);

        var plan = await _suscripcionRepo.ObtenerPorIdAsync(dto.SuscripcionId);
        if (plan is null)
            return (ResultadoSuscripcion.PlanNoEncontrado, null);

        if (await _repo.ObtenerActivaPorAlumnoAsync(alumnoId) is not null)
            return (ResultadoSuscripcion.YaTieneSuscripcionActiva, null);

        var nueva = new AlumnoSuscripcion
        {
            AlumnoId = alumnoId,
            SuscripcionId = plan.Id,
            FechaInicio = DateTime.Now,
            FechaFin = DateTime.Now.AddDays(plan.DuracionDias),
            Activa = true
        };

        await _repo.AgregarAsync(nueva);

        return (ResultadoSuscripcion.Ok, new AlumnoSuscripcionDto(
            nueva.Id, nueva.SuscripcionId, plan.Nombre, plan.Precio,
            nueva.FechaInicio, nueva.FechaFin, nueva.Activa));
    }

    public async Task<ResultadoSuscripcion> CancelarAsync(int alumnoId, int alumnoSuscripcionId)
    {
        var suscripcion = await _repo.ObtenerPorIdAsync(alumnoSuscripcionId);

        if (suscripcion is null || suscripcion.AlumnoId != alumnoId || !suscripcion.Activa)
            return ResultadoSuscripcion.SuscripcionNoEncontrada;

        suscripcion.Activa = false;
        suscripcion.FechaFin = DateTime.Now; // se corta acá, aunque la duración original fuera más larga
        await _repo.ActualizarAsync(suscripcion);

        return ResultadoSuscripcion.Ok;
    }
}