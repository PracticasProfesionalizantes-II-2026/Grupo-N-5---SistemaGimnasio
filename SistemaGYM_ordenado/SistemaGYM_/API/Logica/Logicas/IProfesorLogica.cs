using SistemaGYM.Logica.DTOs;
using SistemaGYM.Entidades;
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
    private readonly IAlumnoRepository _usuarioRepository;

    public ProfesorLogica(IProfesorRepository repository, IAlumnoRepository usuarioRepository)
    {
        _repository = repository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IEnumerable<ProfesorDto>> ObtenerTodosAsync()
    {
        var profesores = await _repository.ObtenerTodosAsync();
        return profesores.Select(p => new ProfesorDto(
            p.Id, p.Dni, p.Nombre, p.Apellido, p.Email, p.Descripcion ?? string.Empty));
    }

    public async Task<ProfesorDto?> ObtenerPorIdAsync(int id)
    {
        var p = await _repository.ObtenerPorIdAsync(id);
        if (p == null) return null;

        return new ProfesorDto(p.Id, p.Dni, p.Nombre, p.Apellido, p.Email, p.Descripcion ?? string.Empty);
    }

    public async Task<ProfesorDetalleDto?> ObtenerDetallePorIdAsync(int id)
    {
        var p = await _repository.ObtenerDetallePorIdAsync(id);
        if (p == null) return null;

        return new ProfesorDetalleDto(
            p.Id, p.Dni, p.Nombre, p.Apellido,
            p.Direccion ?? string.Empty, p.Email, p.Telefono,
            p.Titulo ?? string.Empty, p.Descripcion ?? string.Empty, p.EstaActivo
        );
    }

    public async Task<ProfesorDto> CrearAsync(ProfesorCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Contrasenia))
            throw new ReglaDeNegocioException("La contraseña es obligatoria.");

        await ValidarDatosUnicosAsync(dto.Email, dto.Dni);
        var nuevo = new Profesor
        {
            Dni         = dto.Dni,
            Nombre      = dto.Nombre,
            Apellido    = dto.Apellido,
            Direccion   = dto.Direccion,
            Email       = dto.Email.Trim(),
            Telefono    = dto.Telefono,
            Titulo      = dto.Titulo,
            Descripcion = dto.Descripcion,
            EstaActivo = dto.EstaActivo
        };

        nuevo.SetContrasenia(dto.Contrasenia); 

        await _repository.AgregarAsync(nuevo);
        return new ProfesorDto(nuevo.Id, nuevo.Dni, nuevo.Nombre, nuevo.Apellido, nuevo.Email, nuevo.Descripcion ?? string.Empty);
    }

    public async Task<bool> ActualizarAsync(int id, ProfesorCreateDto dto)
    {
        var p = await _repository.ObtenerPorIdAsync(id);
        if (p == null) return false;

        await ValidarDatosUnicosAsync(dto.Email, dto.Dni, id);

        p.Dni         = dto.Dni;
        p.Nombre      = dto.Nombre;
        p.Apellido    = dto.Apellido;
        p.Direccion   = dto.Direccion;
        p.Email       = dto.Email.Trim();
        p.Telefono    = dto.Telefono;
        p.Titulo      = dto.Titulo;
        p.Descripcion = dto.Descripcion;
        p.EstaActivo  = dto.EstaActivo;

        if (!string.IsNullOrWhiteSpace(dto.Contrasenia))
            p.SetContrasenia(dto.Contrasenia);

        await _repository.ActualizarAsync(p);
        return true;
    }

    private async Task ValidarDatosUnicosAsync(string email, int dni, int? excluirUsuarioId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ReglaDeNegocioException("El email es obligatorio.");
        if (await _usuarioRepository.ExisteEmailODniAsync(email, dni, excluirUsuarioId))
            throw new ReglaDeNegocioException("Ya existe un cliente o profesor registrado con ese email o DNI.");
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var p = await _repository.ObtenerDetallePorIdAsync(id); // trae sus actividades
        if (p == null) return false;

        if (p.Actividades.Any())
            throw new ReglaDeNegocioException("No se puede dar de baja al profesor porque tiene actividades a cargo. Asignalas a otro profesor primero.");

        await _repository.EliminarAsync(p);
        return true;
    }
}
