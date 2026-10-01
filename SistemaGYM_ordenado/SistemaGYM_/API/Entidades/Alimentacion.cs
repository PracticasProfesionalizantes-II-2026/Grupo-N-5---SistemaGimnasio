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
        // Alumnos a los que está asignado. Si no tiene ninguno, es un plan general.
        public ICollection<AlimentacionAlumno> AlimentacionAlumnos { get; set; } = new List<AlimentacionAlumno>();
    }
