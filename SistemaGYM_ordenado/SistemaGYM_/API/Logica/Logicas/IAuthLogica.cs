using SistemaGYM.Logica.DTOs;
using SistemaGYM.Repositorios;

namespace SistemaGYM.Logica;

public enum ResultadoLogin
{
    Ok,
    CredencialesInvalidas
}

public enum ResultadoReset
{
    Ok,
    UsuarioNoEncontrado
}

public interface IAuthLogica
{
    Task<(ResultadoLogin resultado, LoginResultDto? data)> LoginAsync(LoginDto dto);
    Task<ResultadoReset> ResetearContraseniaAsync(ResetPasswordDto dto);
}

public class AuthLogica : IAuthLogica
{
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly IProfesorRepository _profesorRepo;
    private readonly IAdministradorRepository _administradorRepo;

    public AuthLogica(IAlumnoRepository alumnoRepo, IProfesorRepository profesorRepo, IAdministradorRepository administradorRepo)
    {
        _alumnoRepo = alumnoRepo;
        _profesorRepo = profesorRepo;
        _administradorRepo = administradorRepo;
    }

    public async Task<(ResultadoLogin, LoginResultDto?)> LoginAsync(LoginDto dto)
    {
        var alumno = await _alumnoRepo.ObtenerPorEmailAsync(dto.Email);
        if (alumno is { EstaActivo: true } && alumno.VerificarContrasenia(dto.Contrasenia))
            return (ResultadoLogin.Ok, new LoginResultDto(alumno.Id, alumno.Nombre, alumno.Apellido, "Alumno"));

        var profesor = await _profesorRepo.ObtenerPorEmailAsync(dto.Email);
        if (profesor is { EstaActivo: true } && profesor.VerificarContrasenia(dto.Contrasenia))
            return (ResultadoLogin.Ok, new LoginResultDto(profesor.Id, profesor.Nombre, profesor.Apellido, "Profesor"));

        // El campo "Email" también se usa como nombre de usuario para el Administrador
        var admin = await _administradorRepo.ObtenerPorUsuarioAsync(dto.Email);
        if (admin is not null && admin.VerificarContrasenia(dto.Contrasenia))
            return (ResultadoLogin.Ok, new LoginResultDto(admin.Id, "Administrador", string.Empty, "Administrador"));

        return (ResultadoLogin.CredencialesInvalidas, null);
    }

    public async Task<ResultadoReset> ResetearContraseniaAsync(ResetPasswordDto dto)
    {
        var alumno = await _alumnoRepo.ObtenerPorEmailAsync(dto.Email);
        if (alumno is not null && alumno.Dni == dto.Dni)
        {
            alumno.SetContrasenia(dto.NuevaContrasenia);
            await _alumnoRepo.ActualizarAsync(alumno);
            return ResultadoReset.Ok;
        }

        var profesor = await _profesorRepo.ObtenerPorEmailAsync(dto.Email);
        if (profesor is not null && profesor.Dni == dto.Dni)
        {
            profesor.SetContrasenia(dto.NuevaContrasenia);
            await _profesorRepo.ActualizarAsync(profesor);
            return ResultadoReset.Ok;
        }

        return ResultadoReset.UsuarioNoEncontrado;
    }
}
