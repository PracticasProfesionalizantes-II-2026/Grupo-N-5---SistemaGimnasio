namespace SistemaGYM.Entidades;
    public class Alumno: Usuario
    {
        public bool EstaActivo { get; set; }    
          public ICollection<AlumnoSuscripcion> AlumnoSuscripciones { get; set; } = new List<AlumnoSuscripcion>();
          public ICollection<Pago> AlumnoPagos{ get; set; } = new List<Pago>();
          public ICollection<ActividadAlumno> ActividadesAlumno { get; set; } = new List<ActividadAlumno>();

    }