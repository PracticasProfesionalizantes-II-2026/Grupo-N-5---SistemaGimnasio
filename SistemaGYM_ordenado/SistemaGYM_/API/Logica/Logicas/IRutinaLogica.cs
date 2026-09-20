using SistemaGYM.Logica.DTOs;
using SistemaGYM.Entidades;
using SistemaGYM.Repositorios;

namespace SistemaGYM.Logica;

public interface IRutinaLogica
{
    Task<IEnumerable<RutinaDto>> ObtenerTodosAsync();
    Task<RutinaDto?> ObtenerPorIdAsync(int id);
    Task<RutinaDto> CrearAsync(RutinaCreateDto dto);
    Task<bool> ActualizarAsync(int id, RutinaCreateDto dto);
    Task<bool> EliminarAsync(int id);
}

public class RutinaLogica : IRutinaLogica
{
    private readonly IRutinaRepository _repo;
    private readonly IActividadAlumnoRepository _actividadAlumnoRepo;

    public RutinaLogica(IRutinaRepository repo, IActividadAlumnoRepository actividadAlumnoRepo)
    {
        _repo = repo;
        _actividadAlumnoRepo = actividadAlumnoRepo;
    }

    public async Task<IEnumerable<RutinaDto>> ObtenerTodosAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();
        return lista.Select(r => new RutinaDto(
            r.Id, 
            r.Nombre, 
            r.Descripcion, 
            r.ProfesorId, 
            r.AlumnoId,
            r.ActividadId,
            r.Actividad?.Nombre,
            NombreAlumno(r)
        ));
    }

    public async Task<RutinaDto?> ObtenerPorIdAsync(int id)
    {
        var r = await _repo.ObtenerPorIdAsync(id);
        if (r == null) return null;
        
        return new RutinaDto(
            r.Id, 
            r.Nombre, 
            r.Descripcion, 
            r.ProfesorId, 
            r.AlumnoId,
            r.ActividadId,
            r.Actividad?.Nombre,
            NombreAlumno(r)
        );
    }

    public async Task<RutinaDto> CrearAsync(RutinaCreateDto dto)
    {
        if (dto.ActividadId.HasValue && !await _actividadAlumnoRepo.AlumnoEstaInscriptoEnActividadDelProfesorAsync(dto.AlumnoId, dto.ActividadId.Value, dto.ProfesorId))
            throw new ReglaDeNegocioException("Solo podés asignar una rutina a alumnos activos de la actividad seleccionada.");

        var nueva = new Rutina
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            ProfesorId = dto.ProfesorId,
            AlumnoId = dto.AlumnoId,
            ActividadId = dto.ActividadId
        };
        
        await _repo.AgregarAsync(nueva);
        
        return new RutinaDto(
            nueva.Id, 
            nueva.Nombre, 
            nueva.Descripcion, 
            nueva.ProfesorId, 
            nueva.AlumnoId,
            nueva.ActividadId,
            null,
            null
        );
    }

    public async Task<bool> ActualizarAsync(int id, RutinaCreateDto dto)
    {
        var r = await _repo.ObtenerPorIdAsync(id);
        if (r == null) return false;

        if (dto.ActividadId.HasValue && !await _actividadAlumnoRepo.AlumnoEstaInscriptoEnActividadDelProfesorAsync(dto.AlumnoId, dto.ActividadId.Value, dto.ProfesorId))
            throw new ReglaDeNegocioException("Solo podés asignar una rutina a alumnos activos de la actividad seleccionada.");

        r.Nombre = dto.Nombre;
        r.Descripcion = dto.Descripcion;
        r.ProfesorId = dto.ProfesorId;
        r.AlumnoId = dto.AlumnoId;
        r.ActividadId = dto.ActividadId;

        await _repo.ActualizarAsync(r);
        return true;
    }

    private static string? NombreAlumno(Rutina rutina) =>
        rutina.Alumno is null ? null : $"{rutina.Alumno.Nombre} {rutina.Alumno.Apellido}";

    public async Task<bool> EliminarAsync(int id)
    {
        var r = await _repo.ObtenerPorIdAsync(id);
        if (r == null) return false;

        await _repo.EliminarAsync(r);
        return true;
    }
}
