using System;
using System.Collections.Generic;
using System.Text;

namespace ElGas.Models
{
    public partial class Ruta
    {
        public int IdRuta { get; set; }
        public int? IdDistribuidor { get; set; }
        public double? Longitud { get; set; }
        public double? Latitud { get; set; }
    }
}
