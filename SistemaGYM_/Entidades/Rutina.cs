    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
    public class Rutina
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(700)]
        public string Descripcion { get; set; } = string.Empty;
        [Required]
        [MaxLength(70)]
        public string Nombre { get; set; } = string.Empty;
        public int ProfesorId {get; set;}
        [ForeignKey("ProfesorId")]
        public Profesor Profesor { get; set; } = null!;
    }