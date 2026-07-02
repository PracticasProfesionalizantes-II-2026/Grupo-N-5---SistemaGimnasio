using System.ComponentModel.DataAnnotations;
using SistemaGYM;

namespace SistemaGYM.Entidades;
public class Administrador
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Usuario { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Contrasenia {get; set;} = string.Empty;
    public void SetContrasenia(string contraseniaPlana)
        {
            if (string.IsNullOrWhiteSpace(contraseniaPlana))
            throw new ArgumentException("La contraseña no puede estar vacía.", nameof(contraseniaPlana));
            Contrasenia = PasswordHelper.HashPassword(contraseniaPlana);
        }
    public bool VerificarContrasenia(string intento) =>
    PasswordHelper.VerificarPassword(intento, Contrasenia);

}