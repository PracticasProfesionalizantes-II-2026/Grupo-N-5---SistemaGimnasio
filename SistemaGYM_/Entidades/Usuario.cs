using System.ComponentModel.DataAnnotations;

public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(8)]
        public int Dni { get; private set; }
        public string ? Direccion { get; private set; }  
        /* 
        [Required]
        public byte[] PasswordHash { get; set; } = [];
         [Required]
        public byte[] PasswordSalt { get; set; } = [];
        */
        [Required]
        [MaxLength(50)]
        public string Contraseña {get; private set;} = string.Empty;
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

    }