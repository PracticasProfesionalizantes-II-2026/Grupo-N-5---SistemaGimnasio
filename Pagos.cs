using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace PracticaGYM_
{
    public class Pagos
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public MetodoPago _MetodoPago { get; set; }
        public Suscripcion Suscripcion { get; set; }
        public Alumno Alumno { get; set; }
        public enum MetodoPago
        {
            TarjetaCredito,
            TarjetaDebito,
            PayPal,
            TransferenciaBancaria
        }
    }
}
