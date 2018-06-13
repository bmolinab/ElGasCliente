using System;
using System.Collections.Generic;
using System.Text;

namespace ElGas.Models
{
    public class DistribuidorResponse
    {
        public int IdDistribuidor { get; set; }
        public string Identificacion { get; set; }
        public double? Longitud { get; set; }
        public double? Latitud { get; set; }
    }
}
