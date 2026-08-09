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

    public RutinaLogica(IRutinaRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<RutinaDto>> ObtenerTodosAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();
        return lista.Select(r => new RutinaDto(
            r.Id, 
            r.Nombre, 
            r.Descripcion, 
            r.ProfesorId, 
            r.AlumnoId
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
            r.AlumnoId
        );
    }

    public async Task<RutinaDto> CrearAsync(RutinaCreateDto dto)
    {
        var nueva = new Rutina
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            ProfesorId = dto.ProfesorId,
            AlumnoId = dto.AlumnoId
        };
        
        await _repo.AgregarAsync(nueva);
        
        return new RutinaDto(
            nueva.Id, 
            nueva.Nombre, 
            nueva.Descripcion, 
            nueva.ProfesorId, 
            nueva.AlumnoId
        );
    }

    public async Task<bool> ActualizarAsync(int id, RutinaCreateDto dto)
    {
        var r = await _repo.ObtenerPorIdAsync(id);
        if (r == null) return false;

        r.Nombre = dto.Nombre;
        r.Descripcion = dto.Descripcion;
        r.ProfesorId = dto.ProfesorId;
        r.AlumnoId = dto.AlumnoId;

        await _repo.ActualizarAsync(r);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var r = await _repo.ObtenerPorIdAsync(id);
        if (r == null) return false;

        await _repo.EliminarAsync(r);
        return true;
    }
}