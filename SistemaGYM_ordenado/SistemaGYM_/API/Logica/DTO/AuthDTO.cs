namespace SistemaGYM.Logica.DTOs;

public record LoginDto(string Email, string Contrasenia);

public record LoginResultDto(int Id, string Nombre, string Apellido, string Rol);

// Usado por el flujo de "recuperar contraseña": el DNI actúa como verificación de identidad
// dado que el sistema todavía no envía emails/SMS reales con códigos de verificación.
public record ResetPasswordDto(string Email, int Dni, string NuevaContrasenia);
