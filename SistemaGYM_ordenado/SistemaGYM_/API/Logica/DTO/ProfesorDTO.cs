using System.ComponentModel.DataAnnotations;

namespace SistemaGYM.Logica.DTOs; 

public record ProfesorDto(int Id, int Dni, string Nombre, string Apellido, string Email, string Descripcion);

public record ProfesorDetalleDto(
    int Id, 
    int Dni, 
    string Nombre, 
    string Apellido, 
    string Direccion, 
    string Email, 
    string Telefono, 
    string Titulo,
    string Descripcion,
    bool   EstaActivo
    
);


public record ProfesorCreateDto(
    [param: Range(1000000, 99999999, ErrorMessage = "El DNI debe tener 7 u 8 dígitos")]
    int Dni, 
    [param: Required(ErrorMessage = "El nombre es obligatorio")]
    [param: MaxLength(100, ErrorMessage = "El nombre puede tener como máximo 100 caracteres")]
    string Nombre, 
    [param: Required(ErrorMessage = "El apellido es obligatorio")]
    [param: MaxLength(100, ErrorMessage = "El apellido puede tener como máximo 100 caracteres")]
    string Apellido, 
    string Direccion, 
    [param: Required(ErrorMessage = "El email es obligatorio")]
    [param: MaxLength(100, ErrorMessage = "El email puede tener como máximo 100 caracteres")]
    [param: EmailAddress(ErrorMessage = "Ingresá un email válido")]
    string Email, 
    [param: Required(ErrorMessage = "El teléfono es obligatorio")]
    [param: MaxLength(14, ErrorMessage = "El teléfono debe tener como máximo 14 dígitos")]
    [param: RegularExpression("^[0-9]+$", ErrorMessage = "El teléfono solo puede contener números")]
    string Telefono, 
    [param: MaxLength(100, ErrorMessage = "El título puede tener como máximo 100 caracteres")]
    string Titulo,
    [param: MaxLength(500, ErrorMessage = "La descripción puede tener como máximo 500 caracteres")]
    string Descripcion,
    bool EstaActivo,

    // Igual que en AlumnoCreateDto: al modificar se puede dejar vacía para no cambiarla.
    [param: MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    string Contrasenia
);
