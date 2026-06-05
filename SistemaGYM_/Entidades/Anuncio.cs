    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Anuncio
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(70)]
        public string Titulo { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; } = string.Empty;
        [Required]
        public DateTime FechaPublicacion { get; set; } = DateTime.Now;
        public int ProfesorId {get; set;}
        [ForeignKey("ProfesorId")]
         public Profesor Profesor { get; set; } = null!;
    }