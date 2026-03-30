using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaGYM_
{
    public class Actividad
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime Horainicio { get; set; }
        public DateTime Horafin { get; set; }
        public Profesor profesor { get; set; }
        public DiasSemana Dia { get; set; }
        public enum DiasSemana { Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo }
    }
}
