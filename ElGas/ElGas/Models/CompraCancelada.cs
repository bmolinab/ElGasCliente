using System;
using System.Collections.Generic;
using System.Text;

namespace ElGas.Models
{
    public class CompraCancelada
    {
       
            public int IdCompraCancelada { get; set; }

            public int IdCompra { get; set; }

            public int IdCliente { get; set; }

            public int IdDistribuidor { get; set; }

            public DateTime? Fecha { get; set; }

            public int CanceladaPor { get; set; }

                  
    }
}
