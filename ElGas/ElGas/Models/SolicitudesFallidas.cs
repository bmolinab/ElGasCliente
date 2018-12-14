namespace ElGas.Models
{
    using System;

    public partial class SolicitudesFallidas
    {
        public long IdSolicitudesFallidas { get; set; }

        public int IdCliente { get; set; }

        public DateTime FechaHora { get; set; }

        public string Razon { get; set; }

        public double Longitud { get; set; }

        public double Latitud { get; set; }
    }
}
