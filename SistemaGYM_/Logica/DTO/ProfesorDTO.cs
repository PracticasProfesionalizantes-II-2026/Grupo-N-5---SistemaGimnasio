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
    string Descripcion
    
);


public record ProfesorCreateDto(
    int Dni, 
    string Nombre, 
    string Apellido, 
    string Direccion, 
    string Email, 
    string Telefono, 
    string Descripcion,

    [property: Required(ErrorMessage = "La contraseña es obligatoria")]
    [property: MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    string Contrasenia
);