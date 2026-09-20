using SistemaGYM.Logica.DTOs;
using SistemaGYM.Entidades;
using SistemaGYM.Repositorios;

namespace SistemaGYM.Logica;

public interface IAlimentacionLogica
{
    Task<IEnumerable<AlimentacionDto>> ObtenerTodosAsync();
    Task<AlimentacionDto?> ObtenerPorIdAsync(int id);
    Task<AlimentacionDto> CrearAsync(AlimentacionCreateDto dto);
    Task<bool> ActualizarAsync(int id, AlimentacionCreateDto dto);
    Task<bool> EliminarAsync(int id);
}

public class AlimentacionLogica : IAlimentacionLogica
{
    private readonly IAlimentacionRepository _repo;
    private readonly IActividadAlumnoRepository _actividadAlumnoRepo;

    public AlimentacionLogica(IAlimentacionRepository repo, IActividadAlumnoRepository actividadAlumnoRepo)
    {
        _repo = repo;
        _actividadAlumnoRepo = actividadAlumnoRepo;
    }

    public async Task<IEnumerable<AlimentacionDto>> ObtenerTodosAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();
        return lista.Select(a => new AlimentacionDto(
            a.Id, 
            a.TipoAlimentacion, 
            a.Descripcion, 
            a.ProfesorId,
            a.AlumnoId,
            NombreAlumno(a)
        ));
    }

    public async Task<AlimentacionDto?> ObtenerPorIdAsync(int id)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        if (a == null) return null;
        
        return new AlimentacionDto(
            a.Id, 
            a.TipoAlimentacion, 
            a.Descripcion, 
            a.ProfesorId,
            a.AlumnoId,
            NombreAlumno(a)
        );
    }

    public async Task<AlimentacionDto> CrearAsync(AlimentacionCreateDto dto)
    {
        if (dto.AlumnoId.HasValue && !await _actividadAlumnoRepo.AlumnoPerteneceAProfesorAsync(dto.AlumnoId.Value, dto.ProfesorId))
            throw new ReglaDeNegocioException("Solo podés asignar una alimentación a alumnos inscriptos en una actividad del profesor.");

        var nueva = new Alimentacion
        {
            TipoAlimentacion = dto.TipoAlimentacion,
            Descripcion = dto.Descripcion,
            ProfesorId = dto.ProfesorId,
            AlumnoId = dto.AlumnoId
        };
        
        await _repo.AgregarAsync(nueva);
        
        return new AlimentacionDto(
            nueva.Id, 
            nueva.TipoAlimentacion, 
            nueva.Descripcion, 
            nueva.ProfesorId,
            nueva.AlumnoId,
            NombreAlumno(nueva)
        );
    }

    public async Task<bool> ActualizarAsync(int id, AlimentacionCreateDto dto)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        if (a == null) return false;

        a.TipoAlimentacion = dto.TipoAlimentacion;
        a.Descripcion = dto.Descripcion;
        if (dto.AlumnoId.HasValue && !await _actividadAlumnoRepo.AlumnoPerteneceAProfesorAsync(dto.AlumnoId.Value, dto.ProfesorId))
            throw new ReglaDeNegocioException("Solo podés asignar una alimentación a alumnos inscriptos en una actividad del profesor.");
        a.ProfesorId = dto.ProfesorId;
        a.AlumnoId = dto.AlumnoId;

        await _repo.ActualizarAsync(a);
        return true;
    }

    private static string? NombreAlumno(Alimentacion alimentacion) =>
        alimentacion.Alumno is null
            ? null
            : $"{alimentacion.Alumno.Nombre} {alimentacion.Alumno.Apellido}";

    public async Task<bool> EliminarAsync(int id)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        if (a == null) return false;

        await _repo.EliminarAsync(a);
        return true;
    }
}
