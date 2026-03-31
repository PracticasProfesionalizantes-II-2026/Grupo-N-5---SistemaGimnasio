using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaGYM_
{
    public class Alumno: Usuario
    {
        public Suscripcion suscripcion { get; set; }
        public bool EstaActivo { get; set; }    
        public List <Pagos> pagos { get; set; }

    }
}
