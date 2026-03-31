using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaGYM_
{
    public class Alimentacion
    {
        public int Id { get; set; }
        public string TipoAlimentacion { get; set; }    
        public string Descripcion { get; set; }
        public Profesor profesor { get; set; }
    }
}
