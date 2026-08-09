using SistemaGYM.Logica.DTOs;
using SistemaGYM.Repositorios;

namespace SistemaGYM.Logica;

public enum ResultadoLogin
{
    Ok,
    CredencialesInvalidas
}

public interface IAuthLogica
{
    Task<(ResultadoLogin resultado, LoginResultDto? data)> LoginAsync(LoginDto dto);
}

public class AuthLogica : IAuthLogica
{
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly IProfesorRepository _profesorRepo;

    public AuthLogica(IAlumnoRepository alumnoRepo, IProfesorRepository profesorRepo)
    {
        _alumnoRepo = alumnoRepo;
        _profesorRepo = profesorRepo;
    }

    public async Task<(ResultadoLogin, LoginResultDto?)> LoginAsync(LoginDto dto)
    {
        var alumno = await _alumnoRepo.ObtenerPorEmailAsync(dto.Email);
        if (alumno is not null && alumno.VerificarContrasenia(dto.Contrasenia))
            return (ResultadoLogin.Ok, new LoginResultDto(alumno.Id, alumno.Nombre, alumno.Apellido, "Alumno"));

        var profesor = await _profesorRepo.ObtenerPorEmailAsync(dto.Email);
        if (profesor is not null && profesor.VerificarContrasenia(dto.Contrasenia))
            return (ResultadoLogin.Ok, new LoginResultDto(profesor.Id, profesor.Nombre, profesor.Apellido, "Profesor"));

        return (ResultadoLogin.CredencialesInvalidas, null);
    }
}