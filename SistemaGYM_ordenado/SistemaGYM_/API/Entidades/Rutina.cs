using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGYM.Entidades;
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
        // Alumnos a los que está asignada. Si no tiene ninguno, es una rutina general.
        public ICollection<RutinaAlumno> RutinaAlumnos { get; set; } = new List<RutinaAlumno>();
        // Nulo para conservar las rutinas históricas cargadas sin actividad.
        public int? ActividadId { get; set; }
        [ForeignKey("ActividadId")]
        public Actividad? Actividad { get; set; }
    }
