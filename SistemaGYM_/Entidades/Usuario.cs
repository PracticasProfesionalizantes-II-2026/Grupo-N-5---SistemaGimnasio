using System.ComponentModel.DataAnnotations;
using SistemaGYM;

namespace SistemaGYM.Entidades;
public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1000000, 99999999, ErrorMessage = "El DNI debe tener entre 7 y 8 dígitos")]
        public int Dni { get; set; }
        public string ? Direccion { get; set; }  

        [Required]
        [MaxLength(100)]
        public string Contrasenia {get; private set;} = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Apellido { get;    set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }  = string.Empty;
        [Required]
        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;
        public DateTime FechaAlta { get; set; } = DateTime.Now;


        public void SetContrasenia(string contraseniaPlana)
            {
                if (string.IsNullOrWhiteSpace(contraseniaPlana))
                throw new ArgumentException("La contraseña no puede estar vacía.", nameof(contraseniaPlana));

                Contrasenia = PasswordHelper.HashPassword(contraseniaPlana);
            }
        public bool VerificarContrasenia(string intento) =>
        PasswordHelper.VerificarPassword(intento, Contrasenia);
    }