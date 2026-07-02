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

    public ActividadLogica(IActividadRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<ActividadDto>> ObtenerTodosAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();
        return lista.Select(a => new ActividadDto(
            a.ActividadId, 
            a.Nombre, 
            a.Descripcion, 
            a.HoraInicio, 
            a.HoraFin, 
            a.ProfesorId, 
            a.Dias
        ));
    }

    public async Task<ActividadDto?> ObtenerPorIdAsync(int id)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        if (a == null) return null;
        
        return new ActividadDto(
            a.ActividadId, 
            a.Nombre, 
            a.Descripcion, 
            a.HoraInicio, 
            a.HoraFin, 
            a.ProfesorId, 
            a.Dias
        );
    }

    public async Task<ActividadDto> CrearAsync(ActividadCreateDto dto)
    {
        var nueva = new Actividad
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            HoraInicio = dto.HoraInicio, 
            HoraFin = dto.HoraFin,       
            ProfesorId = dto.ProfesorId,
            Dias = dto.Dias
        };
        
        await _repo.AgregarAsync(nueva);
        
        return new ActividadDto(
            nueva.ActividadId, 
            nueva.Nombre, 
            nueva.Descripcion, 
            nueva.HoraInicio, 
            nueva.HoraFin, 
            nueva.ProfesorId, 
            nueva.Dias
        );
    }

    public async Task<bool> ActualizarAsync(int id, ActividadCreateDto dto)
    {
        var a = await _repo.ObtenerPorIdAsync(id);
        if (a == null) return false;

        a.Nombre = dto.Nombre;
        a.Descripcion = dto.Descripcion;
        a.HoraInicio = dto.HoraInicio;
        a.HoraFin = dto.HoraFin;
        a.ProfesorId = dto.ProfesorId;
        a.Dias = dto.Dias;

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