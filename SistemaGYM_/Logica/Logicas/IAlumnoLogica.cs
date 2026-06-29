using SistemaGYM.Logica.DTOs;
using SistemaGYM.Entidades;
using SistemaGYM.Repositorios;

namespace SistemaGYM.Logica;

public interface IAlumnoLogica
{
    Task<IEnumerable<AlumnoDto>> ObtenerTodosAsync();
    Task<AlumnoDto?> ObtenerPorIdAsync(int id);
    Task<AlumnoDetalleDto?> ObtenerDetallePorIdAsync(int id);
    Task<AlumnoDto> CrearAsync(AlumnoCreateDto dto);
    Task<bool> ActualizarAsync(int id, AlumnoCreateDto dto);
    Task<bool> EliminarAsync(int id);
}

public class AlumnoLogica : IAlumnoLogica
{
    private readonly IAlumnoRepository _repository;

    public AlumnoLogica(IAlumnoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AlumnoDto>> ObtenerTodosAsync()
    {
        var alumnos = await _repository.ObtenerTodosAsync();
        return alumnos.Select(a => new AlumnoDto(a.Id, a.Dni, a.Nombre, a.Apellido));
    }

    public async Task<AlumnoDto?> ObtenerPorIdAsync(int id)
    {
        var a = await _repository.ObtenerPorIdAsync(id);
        return a == null ? null : new AlumnoDto(a.Id, a.Dni, a.Nombre, a.Apellido);
    }

    public async Task<AlumnoDetalleDto?> ObtenerDetallePorIdAsync(int id)
    {
        // Usa el método con Include para traer la suscripción activa
        var a = await _repository.ObtenerDetallePorIdAsync(id);
        if (a == null) return null;

        // Nombre de la suscripción activa, o "Sin suscripción" si no tiene
        var suscripcionActiva = a.AlumnoSuscripciones
            .FirstOrDefault(s => s.Activa)
            ?.Suscripcion.Nombre ?? "Sin suscripción";

        return new AlumnoDetalleDto(
            a.Id,
            a.Dni,
            a.Nombre,
            a.Apellido,
            a.Direccion ?? string.Empty,
            a.Email,
            a.Telefono,             // ya es string, sin conversión
            suscripcionActiva,
            a.EstaActivo.ToString()
        );
    }

    public async Task<AlumnoDto> CrearAsync(AlumnoCreateDto dto)
    {
        var nuevo = new Alumno
        {
            Nombre     = dto.Nombre,
            Apellido   = dto.Apellido,
            Email      = dto.Email,
            Telefono   = dto.Telefono,      // ya es string, sin .ToString()
            EstaActivo = dto.EstaActivo     // ya es bool, sin bool.Parse()
        };

        // Dni y Direccion tienen private set en Usuario.
        // Solución temporal hasta cambiar private set → set en Usuario.cs
        nuevo.GetType().GetProperty("Dni")!.SetValue(nuevo, dto.Dni);
        nuevo.GetType().GetProperty("Direccion")!.SetValue(nuevo, dto.Direccion);

        await _repository.AgregarAsync(nuevo);
        return new AlumnoDto(nuevo.Id, nuevo.Dni, nuevo.Nombre, nuevo.Apellido);
    }

    public async Task<bool> ActualizarAsync(int id, AlumnoCreateDto dto)
    {
        var a = await _repository.ObtenerPorIdAsync(id);
        if (a == null) return false;

        a.Nombre     = dto.Nombre;
        a.Apellido   = dto.Apellido;
        a.Email      = dto.Email;
        a.Telefono   = dto.Telefono;        // ya es string
        a.EstaActivo = dto.EstaActivo;      // ya es bool

        // Igual que CrearAsync: Dni y Direccion tienen private set
        a.GetType().GetProperty("Dni")!.SetValue(a, dto.Dni);
        a.GetType().GetProperty("Direccion")!.SetValue(a, dto.Direccion);

        await _repository.ActualizarAsync(a);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var a = await _repository.ObtenerPorIdAsync(id);
        if (a == null) return false;

        await _repository.EliminarAsync(a);
        return true;
    }
}
