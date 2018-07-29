using System;
using System.Collections.Generic;
using System.Text;

namespace ElGas.Models
{
    public class Distribuidor
    {
        public int IdDistribuidor { get; set; }

        public string Identificacion { get; set; }

        public string Nombres { get; set; }

        public string Apellidos { get; set; }

        public string Telefono { get; set; }

        public string Correo { get; set; }

        public string PlacaVehiculo { get; set; }

        public int? Prioridad { get; set; }

        public int? IdTipoSuscripcion { get; set; }

        public string IdAspNetUser { get; set; }

        public string DeviceID { get; set; }
        public bool Habilitado { get; set; }
    }

}
