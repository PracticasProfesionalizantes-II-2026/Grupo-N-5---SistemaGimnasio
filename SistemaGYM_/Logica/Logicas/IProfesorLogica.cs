using SistemaGYM.Logica.DTOs;
using SistemaGYM.Repositorios;

namespace SistemaGYM.Logica;

public interface IProfesorLogica
{
    Task<IEnumerable<ProfesorDto>> ObtenerTodosAsync();
    Task<ProfesorDto?> ObtenerPorIdAsync(int id);
    Task<ProfesorDetalleDto?> ObtenerDetallePorIdAsync(int id);
    Task<ProfesorDto> CrearAsync(ProfesorCreateDto dto);
    Task<bool> ActualizarAsync(int id, ProfesorCreateDto dto);
    Task<bool> EliminarAsync(int id);
}

public class ProfesorLogica : IProfesorLogica
{
    private readonly IProfesorRepository _repository;

    public ProfesorLogica(IProfesorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProfesorDto>> ObtenerTodosAsync()
    {
        var profesores = await _repository.ObtenerTodosAsync();
        return profesores.Select(p => new ProfesorDto(
            p.Id,
            p.Dni,
            p.Nombre,
            p.Apellido,
            p.Email,
            p.Descripcion ?? string.Empty
        ));
    }

    public async Task<ProfesorDto?> ObtenerPorIdAsync(int id)
    {
        var p = await _repository.ObtenerPorIdAsync(id);
        if (p == null) return null;

        return new ProfesorDto(
            p.Id,
            p.Dni,
            p.Nombre,
            p.Apellido,
            p.Email,
            p.Descripcion ?? string.Empty
        );
    }

    public async Task<ProfesorDetalleDto?> ObtenerDetallePorIdAsync(int id)
    {
        var p = await _repository.ObtenerDetallePorIdAsync(id);
        if (p == null) return null;

        return new ProfesorDetalleDto(
            p.Id,
            p.Dni,
            p.Nombre,
            p.Apellido,
            p.Direccion ?? string.Empty,
            p.Email,
            long.Parse(p.Telefono),
            p.Descripcion ?? string.Empty
        );
    }

    public async Task<ProfesorDto> CrearAsync(ProfesorCreateDto dto)
    {
        var nuevo = new Profesor
        {
            Dni      = dto.Dni,
            Nombre   = dto.Nombre,
            Apellido = dto.Apellido,
            Direccion = dto.Direccion,
            Email    = dto.Email,
            Telefono = dto.Telefono.ToString(),
            Descripcion = dto.Descripcion
        };

        await _repository.AgregarAsync(nuevo);

        return new ProfesorDto(
            nuevo.Id,
            nuevo.Dni,
            nuevo.Nombre,
            nuevo.Apellido,
            nuevo.Email,
            nuevo.Descripcion ?? string.Empty
        );
    }

    public async Task<bool> ActualizarAsync(int id, ProfesorCreateDto dto)
    {
        var p = await _repository.ObtenerPorIdAsync(id);
        if (p == null) return false;

        p.Dni       = dto.Dni;
        p.Nombre    = dto.Nombre;
        p.Apellido  = dto.Apellido;
        p.Direccion = dto.Direccion;
        p.Email     = dto.Email;
        p.Telefono  = dto.Telefono.ToString();
        p.Descripcion = dto.Descripcion;

        await _repository.ActualizarAsync(p);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var p = await _repository.ObtenerPorIdAsync(id);
        if (p == null) return false;

        await _repository.EliminarAsync(p);
        return true;
    }
}
