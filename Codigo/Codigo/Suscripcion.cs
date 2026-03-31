using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaGYM_
{
    public class Suscripcion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } 
        public decimal Precio { get; set; }
        public string CantidadDias { get; set; }
        public bool EstaActiva { get; set; }
        public List <Alumno> Alumnos { get; set; }
    }
}
