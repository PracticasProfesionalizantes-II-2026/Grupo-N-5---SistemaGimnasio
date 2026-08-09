using SistemaGYM.Logica.DTOs;
using SistemaGYM.Entidades;
using SistemaGYM.Repositorios;

namespace SistemaGYM.Logica;

public interface IAnuncioLogica
{
    Task<IEnumerable<AnuncioDto>> ObtenerTodosAsync();
    Task<AnuncioDto?> ObtenerPorIdAsync(int id);
    Task<AnuncioDto> CrearAsync(AnuncioCreateDto dto);
    Task<bool> ActualizarAsync(int id, AnuncioCreateDto dto);
    Task<bool> EliminarAsync(int id);
}

public class AnuncioLogica : IAnuncioLogica
{
    private readonly IAnuncioRepository _repo;

    public AnuncioLogica(IAnuncioRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<AnuncioDto>> ObtenerTodosAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();
        return lista.Select(a => new AnuncioDto(
            a.Id, 
            a.Titulo, 
            a.Descripcion, 
            a.FechaPublicacion, 
            a.ProfesorId
        ));
    }

    public async Task<AnuncioDto?> ObtenerPorIdAsync(int id)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        if (a == null) return null;
        
        return new AnuncioDto(
            a.Id, 
            a.Titulo, 
            a.Descripcion, 
            a.FechaPublicacion, 
            a.ProfesorId
        );
    }

    public async Task<AnuncioDto> CrearAsync(AnuncioCreateDto dto)
    {
        var nuevo = new Anuncio
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            FechaPublicacion = dto.FechaPublicacion,
            ProfesorId = dto.ProfesorId
        };
        
        await _repo.AgregarAsync(nuevo);
        
        return new AnuncioDto(
            nuevo.Id, 
            nuevo.Titulo, 
            nuevo.Descripcion, 
            nuevo.FechaPublicacion, 
            nuevo.ProfesorId
        );
    }

    public async Task<bool> ActualizarAsync(int id, AnuncioCreateDto dto)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        if (a == null) return false;

        a.Titulo = dto.Titulo;
        a.Descripcion = dto.Descripcion;
        a.FechaPublicacion = dto.FechaPublicacion;
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