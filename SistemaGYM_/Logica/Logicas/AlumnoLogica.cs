using SistemaGYM.Logica.DTOs;
using SistemaGYM.Repositorios;
 
namespace SistemaGYM.Logica;
 
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
        var a = await _repository.ObtenerPorIdAsync(id);
        if (a == null) return null;
 
        return new AlumnoDetalleDto(
            a.Id,
            a.Dni,
            a.Nombre,
            a.Apellido,
            a.Direccion ?? string.Empty,
            a.Email,
            Convert.ToInt64(a.Telefono),
            a.Suscripcion,
            a.EstaActivo.ToString()
        );
    }
 
    public async Task<AlumnoDto> CrearAsync(AlumnoCreateDto dto)
    {
        var nuevo = new Alumno
        {
            Dni = dto.Dni,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Direccion = dto.Direccion,
            Email = dto.Email,
            Telefono = dto.Telefono.ToString(),
            Suscripcion = dto.Suscripcion,
            EstaActivo = bool.Parse(dto.EstaActivo)
        };
 
        await _repository.AgregarAsync(nuevo);
        return new AlumnoDto(nuevo.Id, nuevo.Dni, nuevo.Nombre, nuevo.Apellido);
    }
 
    public async Task<bool> ActualizarAsync(int id, AlumnoCreateDto dto)
    {
        var a = await _repository.ObtenerPorIdAsync(id);
        if (a == null) return false;
 
        a.Dni = dto.Dni;
        a.Nombre = dto.Nombre;
        a.Apellido = dto.Apellido;
        a.Direccion = dto.Direccion;
        a.Email = dto.Email;
        a.Telefono = dto.Telefono.ToString();
        a.Suscripcion = dto.Suscripcion;
        a.EstaActivo = bool.Parse(dto.EstaActivo);
 
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