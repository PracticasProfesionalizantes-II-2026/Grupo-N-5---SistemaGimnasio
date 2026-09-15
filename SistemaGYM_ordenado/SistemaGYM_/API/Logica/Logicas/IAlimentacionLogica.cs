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
        return lista.Select(a => new AlimentacionDto(
            a.Id, 
            a.TipoAlimentacion, 
            a.Descripcion, 
            a.ProfesorId
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
            a.ProfesorId
        );
    }

    public async Task<AlimentacionDto> CrearAsync(AlimentacionCreateDto dto)
    {
        var nueva = new Alimentacion
        {
            TipoAlimentacion = dto.TipoAlimentacion,
            Descripcion = dto.Descripcion,
            ProfesorId = dto.ProfesorId
        };
        
        await _repo.AgregarAsync(nueva);
        
        return new AlimentacionDto(
            nueva.Id, 
            nueva.TipoAlimentacion, 
            nueva.Descripcion, 
            nueva.ProfesorId
        );
    }

    public async Task<bool> ActualizarAsync(int id, AlimentacionCreateDto dto)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        if (a == null) return false;

        a.TipoAlimentacion = dto.TipoAlimentacion;
        a.Descripcion = dto.Descripcion;
        a.ProfesorId = dto.ProfesorId;

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
}