using System.ComponentModel.DataAnnotations;

public class Administrador
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Usuario { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string Contraseña {get; set;} = string.Empty;

    /*[Required]
    public byte[] PasswordHash { get; set; } = [];

    [Required]
    public byte[] PasswordSalt { get; set; } = [];
    */
}