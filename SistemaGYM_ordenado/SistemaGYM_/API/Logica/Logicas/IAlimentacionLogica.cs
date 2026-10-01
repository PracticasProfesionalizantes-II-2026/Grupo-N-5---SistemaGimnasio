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

    public AlimentacionLogica(IAlimentacionRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<AlimentacionDto>> ObtenerTodosAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();
        return lista.Select(a => ADto(a));
    }

    public async Task<AlimentacionDto?> ObtenerPorIdAsync(int id)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        return a == null ? null : ADto(a);
    }

    // Un solo plan para todos los alumnos elegidos (sin alumnos = plan general)
    public async Task<AlimentacionDto> CrearAsync(AlimentacionCreateDto dto)
    {
        var alumnoIds = (dto.AlumnoIds ?? new List<int>()).Distinct().ToList();

        var nueva = new Alimentacion
        {
            TipoAlimentacion = dto.TipoAlimentacion,
            Descripcion = dto.Descripcion,
            ProfesorId = dto.ProfesorId,
            AlimentacionAlumnos = alumnoIds.Select(id => new AlimentacionAlumno { AlumnoId = id }).ToList()
        };

        await _repo.AgregarAsync(nueva);
        return ADto(nueva);
    }

    // Al modificar el plan, el cambio lo ven todos los alumnos asignados
    public async Task<bool> ActualizarAsync(int id, AlimentacionCreateDto dto)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        if (a == null) return false;

        var alumnoIds = (dto.AlumnoIds ?? new List<int>()).Distinct().ToList();

        a.TipoAlimentacion = dto.TipoAlimentacion;
        a.Descripcion = dto.Descripcion;
        a.ProfesorId = dto.ProfesorId;

        // Se quitan los alumnos que ya no están elegidos y se agregan los nuevos
        foreach (var asignacion in a.AlimentacionAlumnos.Where(aa => !alumnoIds.Contains(aa.AlumnoId)).ToList())
            a.AlimentacionAlumnos.Remove(asignacion);
        foreach (var alumnoId in alumnoIds.Where(x => a.AlimentacionAlumnos.All(aa => aa.AlumnoId != x)))
            a.AlimentacionAlumnos.Add(new AlimentacionAlumno { AlumnoId = alumnoId });

        await _repo.ActualizarAsync(a);
        return true;
    }

    private static AlimentacionDto ADto(Alimentacion a) => new(
        a.Id,
        a.TipoAlimentacion,
        a.Descripcion,
        a.ProfesorId,
        a.AlimentacionAlumnos
            .Where(aa => aa.Alumno != null)
            .Select(aa => new AlumnoAsignadoDto(aa.AlumnoId, $"{aa.Alumno.Nombre} {aa.Alumno.Apellido}"))
            .OrderBy(x => x.Nombre)
            .ToList()
    );

    public async Task<bool> EliminarAsync(int id)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        if (a == null) return false;

        await _repo.EliminarAsync(a);   // sus asignaciones a alumnos se borran en cascada
        return true;
    }
}
