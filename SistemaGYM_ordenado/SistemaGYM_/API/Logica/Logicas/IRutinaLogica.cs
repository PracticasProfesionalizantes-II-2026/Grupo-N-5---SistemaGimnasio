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
    private readonly IActividadRepository _actividadRepo;
    private readonly IActividadAlumnoRepository _actividadAlumnoRepo;

    public RutinaLogica(IRutinaRepository repo, IActividadRepository actividadRepo, IActividadAlumnoRepository actividadAlumnoRepo)
    {
        _repo = repo;
        _actividadRepo = actividadRepo;
        _actividadAlumnoRepo = actividadAlumnoRepo;
    }

    public async Task<IEnumerable<RutinaDto>> ObtenerTodosAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();
        return lista.Select(r => ADto(r));
    }

    public async Task<RutinaDto?> ObtenerPorIdAsync(int id)
    {
        var r = await _repo.ObtenerPorIdAsync(id);
        return r == null ? null : ADto(r);
    }

    // Una sola rutina para todos los alumnos elegidos (sin alumnos = rutina general)
    public async Task<RutinaDto> CrearAsync(RutinaCreateDto dto)
    {
        var alumnoIds = (dto.AlumnoIds ?? new List<int>()).Distinct().ToList();
        await ValidarAsync(dto.ProfesorId, dto.ActividadId, alumnoIds);

        var nueva = new Rutina
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            ProfesorId = dto.ProfesorId,
            ActividadId = dto.ActividadId,
            RutinaAlumnos = alumnoIds.Select(id => new RutinaAlumno { AlumnoId = id }).ToList()
        };

        await _repo.AgregarAsync(nueva);
        return ADto(nueva);
    }

    // Al modificar la rutina, el cambio lo ven todos los alumnos asignados
    public async Task<bool> ActualizarAsync(int id, RutinaCreateDto dto)
    {
        var r = await _repo.ObtenerPorIdAsync(id);
        if (r == null) return false;

        var alumnoIds = (dto.AlumnoIds ?? new List<int>()).Distinct().ToList();
        await ValidarAsync(dto.ProfesorId, dto.ActividadId, alumnoIds);

        r.Nombre = dto.Nombre;
        r.Descripcion = dto.Descripcion;
        r.ProfesorId = dto.ProfesorId;
        r.ActividadId = dto.ActividadId;

        // Se quitan los alumnos que ya no están elegidos y se agregan los nuevos
        foreach (var asignacion in r.RutinaAlumnos.Where(ra => !alumnoIds.Contains(ra.AlumnoId)).ToList())
            r.RutinaAlumnos.Remove(asignacion);
        foreach (var alumnoId in alumnoIds.Where(a => r.RutinaAlumnos.All(ra => ra.AlumnoId != a)))
            r.RutinaAlumnos.Add(new RutinaAlumno { AlumnoId = alumnoId });

        await _repo.ActualizarAsync(r);
        return true;
    }

    // Si la rutina es de una actividad, la actividad tiene que ser del profesor
    // y los alumnos elegidos tienen que estar inscriptos en ella.
    private async Task ValidarAsync(int profesorId, int? actividadId, List<int> alumnoIds)
    {
        if (!actividadId.HasValue) return;

        var actividad = await _actividadRepo.ObtenerPorIdAsync(actividadId.Value);
        if (actividad == null || actividad.ProfesorId != profesorId)
            throw new ReglaDeNegocioException("La actividad elegida no está a cargo del profesor de la rutina.");

        foreach (var alumnoId in alumnoIds)
            if (!await _actividadAlumnoRepo.AlumnoEstaInscriptoEnActividadDelProfesorAsync(alumnoId, actividadId.Value, profesorId))
                throw new ReglaDeNegocioException("Solo podés asignar la rutina a alumnos inscriptos en la actividad elegida.");
    }

    private static RutinaDto ADto(Rutina r) => new(
        r.Id,
        r.Nombre,
        r.Descripcion,
        r.ProfesorId,
        r.ActividadId,
        r.Actividad?.Nombre,
        r.RutinaAlumnos
            .Where(ra => ra.Alumno != null)
            .Select(ra => new AlumnoAsignadoDto(ra.AlumnoId, $"{ra.Alumno.Nombre} {ra.Alumno.Apellido}"))
            .OrderBy(a => a.Nombre)
            .ToList()
    );

    public async Task<bool> EliminarAsync(int id)
    {
        var r = await _repo.ObtenerPorIdAsync(id);
        if (r == null) return false;

        await _repo.EliminarAsync(r);   // sus asignaciones a alumnos se borran en cascada
        return true;
    }
}
