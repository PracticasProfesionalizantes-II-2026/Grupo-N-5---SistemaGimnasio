using System.ComponentModel.DataAnnotations;

namespace SistemaGYM.Logica.DTOs;

public record LoginDto(
    // También recibe el nombre de usuario del administrador (por ejemplo, Admin123).
    [Required] string Email,
    [Required, MinLength(6)] string Contrasenia);

public record LoginResultDto(int Id, string Nombre, string Apellido, string Rol);

// Usado por el flujo de "recuperar contraseña": el DNI actúa como verificación de identidad
// dado que el sistema todavía no envía emails/SMS reales con códigos de verificación.
public record ResetPasswordDto(
    [Required, EmailAddress] string Email,
    [Range(1_000_000, 99_999_999)] int Dni,
    [Required, MinLength(6)] string NuevaContrasenia);
