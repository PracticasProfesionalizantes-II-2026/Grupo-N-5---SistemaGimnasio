using SistemaGYM.Logica.DTOs;
using SistemaGYM.Entidades;
using SistemaGYM.Repositorios;

namespace SistemaGYM.Logica;

public interface IActividadLogica
{
    Task<IEnumerable<ActividadDto>> ObtenerTodosAsync();
    Task<ActividadDto?> ObtenerPorIdAsync(int id);
    Task<ActividadDto> CrearAsync(ActividadCreateDto dto);
    Task<bool> ActualizarAsync(int id, ActividadCreateDto dto);
    Task<bool> EliminarAsync(int id);
}

public class ActividadLogica : IActividadLogica
{
    private readonly IActividadRepository _repo;
    private readonly IProfesorRepository _profesorRepo;

    public ActividadLogica(IActividadRepository repo, IProfesorRepository profesorRepo)
    {
        _repo = repo;
        _profesorRepo = profesorRepo;
    }

    public async Task<IEnumerable<ActividadDto>> ObtenerTodosAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();
        return lista.Select(a => ADto(a));
    }

    public async Task<ActividadDto?> ObtenerPorIdAsync(int id)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        return a == null ? null : ADto(a);
    }

    public async Task<ActividadDto> CrearAsync(ActividadCreateDto dto)
    {
        await ValidarAsync(dto);

        var nueva = new Actividad
        {
            Nombre = dto.Nombre.Trim(),
            Descripcion = dto.Descripcion,
            HoraInicio = dto.HoraInicio, 
            HoraFin = dto.HoraFin,       
            ProfesorId = dto.ProfesorId,
            Dias = dto.Dias,
            Cupo = dto.Cupo
        };
        
        await _repo.AgregarAsync(nueva);
        return ADto(nueva);
    }

    public async Task<bool> ActualizarAsync(int id, ActividadCreateDto dto)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        if (a == null) return false;

        await ValidarAsync(dto, id);

        var inscriptos = a.ActividadesAlumno.Count(x => x.Activa);
        if (dto.Cupo < inscriptos)
            throw new ReglaDeNegocioException($"El cupo no puede ser menor a los {inscriptos} alumnos ya inscriptos.");

        a.Nombre = dto.Nombre.Trim();
        a.Descripcion = dto.Descripcion;
        a.HoraInicio = dto.HoraInicio;
        a.HoraFin = dto.HoraFin;
        a.ProfesorId = dto.ProfesorId;
        a.Dias = dto.Dias;
        a.Cupo = dto.Cupo;

        await _repo.ActualizarAsync(a);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        if (a == null) return false;

        await _repo.EliminarAsync(a);
        return true;
    }

    // Reglas que valen tanto al crear como al modificar una actividad
    private async Task ValidarAsync(ActividadCreateDto dto, int? actividadId = null)
    {
        if (dto.HoraInicio >= dto.HoraFin)
            throw new ReglaDeNegocioException("La hora de inicio debe ser anterior a la hora de finalización.");

        if (await _repo.ExisteNombreAsync(dto.Nombre, actividadId))
            throw new ReglaDeNegocioException($"Ya existe una actividad llamada \"{dto.Nombre.Trim()}\".");

        // Solo se puede asignar un profesor que exista y esté activo
        var profesor = await _profesorRepo.ObtenerPorIdAsync(dto.ProfesorId);
        if (profesor is null || !profesor.EstaActivo)
            throw new ReglaDeNegocioException("La actividad debe estar a cargo de un profesor activo.");
    }

    private static ActividadDto ADto(Actividad a) => new(
        a.ActividadId,
        a.Nombre,
        a.Descripcion,
        a.HoraInicio,
        a.HoraFin,
        a.ProfesorId,
        a.Dias,
        a.Cupo,
        a.ActividadesAlumno.Count(x => x.Activa)
    );
}
