    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using SistemaGYM;

    namespace SistemaGYM.Entidades;
    public class Alimentacion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string TipoAlimentacion { get; set; }   = string.Empty;
        [Required]
        [MaxLength(700)]
        public string Descripcion { get; set; } = string.Empty;
        public int ProfesorId {get; set;}
        [ForeignKey("ProfesorId")]
        public Profesor Profesor { get; set; } = null!;
        // Nulo únicamente para conservar planes genéricos ya cargados.
        public int? AlumnoId { get; set; }
        [ForeignKey("AlumnoId")]
        public Alumno? Alumno { get; set; }
    }
