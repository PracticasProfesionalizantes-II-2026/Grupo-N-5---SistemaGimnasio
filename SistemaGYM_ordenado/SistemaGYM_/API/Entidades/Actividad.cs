using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SistemaGYM;

namespace SistemaGYM.Entidades;
    public class Actividad
    {
        [Key]
        public int ActividadId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; } = string.Empty;
        [Required]
        public TimeOnly HoraInicio { get; set; }
        [Required]
        public TimeOnly HoraFin { get; set; }
        public int ProfesorId {get; set;}
        [ForeignKey("ProfesorId")]
        public Profesor Profesor { get; set; } = null!;
        [Required]
        public DiasSemana Dias { get; set; }
        // Cantidad máxima de alumnos que pueden inscribirse a la actividad.
        [Range(1, 500)]
        public int Cupo { get; set; } = 20;
        public ICollection<ActividadAlumno> ActividadesAlumno { get; set; } = new List<ActividadAlumno>();
    }