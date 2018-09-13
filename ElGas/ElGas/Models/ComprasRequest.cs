using System;
using System.Collections.Generic;
using System.Text;

namespace ElGas.Models
{
    public class ComprasRequest
    {        
        public int IdCompra { get; set; }
        public int? IdCliente { get; set; }
        public int? IdDistribuidor { get; set; }
        public int? Estado { get; set; }

        public double? Longitud { get; set; }
        public double? Latitud { get; set; }

        public int? Cantidad { get; set; }
        public double? ValorTotal { get; set; }
        public DateTime? FechaPedido { get; set; }

        public string TituloPedido { get; set; }
        public string EstadoPedido { get; set; }

        public bool isCancelable { get; set; }

        public string FechaTexto { get; set; }

        public string icono { get; set; }

        public string Direccion { get; set; }
        public string Referencia { get; set; }
    }

}
