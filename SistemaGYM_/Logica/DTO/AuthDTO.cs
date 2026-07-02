namespace SistemaGYM.Logica.DTOs;

public record LoginDto(string Email, string Contrasenia);

public record LoginResultDto(int Id, string Nombre, string Apellido, string Rol);