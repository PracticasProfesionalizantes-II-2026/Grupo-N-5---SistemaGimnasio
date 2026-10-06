using System.ComponentModel.DataAnnotations;

namespace SistemaGYM.Logica.DTOs;

public record LoginDto(
    // También recibe el nombre de usuario del administrador (por ejemplo, Admin123).
    [Required(ErrorMessage = "El email es obligatorio")] string Email,
    [Required(ErrorMessage = "La contraseña es obligatoria"), MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")] string Contrasenia);

public record LoginResultDto(int Id, string Nombre, string Apellido, string Rol);

// Usado por el flujo de "recuperar contraseña": el DNI actúa como verificación de identidad
// dado que el sistema todavía no envía emails/SMS reales con códigos de verificación.
public record ResetPasswordDto(
    [Required(ErrorMessage = "El email es obligatorio"), EmailAddress(ErrorMessage = "El email no tiene un formato válido")] string Email,
    [Range(1_000_000, 99_999_999, ErrorMessage = "El DNI debe tener 7 u 8 dígitos")] int Dni,
    [Required(ErrorMessage = "La contraseña es obligatoria"), MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")] string NuevaContrasenia);
