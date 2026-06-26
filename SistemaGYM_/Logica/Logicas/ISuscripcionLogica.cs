using SistemaGYM.Logica.DTOs;
using SistemaGYM.Entidades;
using SistemaGYM.Repositorios;

namespace SistemaGYM.Logica;

public interface ISuscripcionLogica
{
    Task<IEnumerable<SuscripcionDto>> ObtenerTodosAsync();
    Task<SuscripcionDto?> ObtenerPorIdAsync(int id);
    Task<SuscripcionDto> CrearAsync(SuscripcionCreateDto dto);
    Task<bool> ActualizarAsync(int id, SuscripcionCreateDto dto);
    Task<bool> EliminarAsync(int id);
}

public class SuscripcionLogica : ISuscripcionLogica
{
    private readonly ISuscripcionRepository _repo;

    public SuscripcionLogica(ISuscripcionRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<SuscripcionDto>> ObtenerTodosAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();
        return lista.Select(s => new SuscripcionDto(
            s.Id,
            "Suscripción Activa" 
        ));
    }

    public async Task<SuscripcionDto?> ObtenerPorIdAsync(int id)
    {
        var s = await _repo.ObtenerPorIdAsync(id);
        if (s == null) return null;
        
        return new SuscripcionDto(
            s.Id,
            "Suscripción Activa"
        );
    }

    public async Task<SuscripcionDto> CrearAsync(SuscripcionCreateDto dto)
    {
        var nueva = new Suscripcion(); 
        
        await _repo.AgregarAsync(nueva);
        
        return new SuscripcionDto(
            nueva.Id,
            dto.Descripcion
        );
    }

    public async Task<bool> ActualizarAsync(int id, SuscripcionCreateDto dto)
    {
        var s = await _repo.ObtenerPorIdAsync(id);
        if (s == null) return false;


        await _repo.ActualizarAsync(s);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var s = await _repo.ObtenerPorIdAsync(id);
        if (s == null) return false;

        await _repo.EliminarAsync(s);
        return true;
    }
}