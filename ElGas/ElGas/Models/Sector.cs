using System;
namespace ElGas.Models
{
    public class Sector
    {
        public string Id
        {
            get;
            set;
        }

        public string IdCiudad
        {
            get;
            set;
        }

        public string Nombre
        {
            get;
            set;
        }

        public Sector()
        {

        }

        public Sector(string id, string idCiudad, string nombre)
        {
            this.Id = id;
            this.IdCiudad = idCiudad;
            this.Nombre = nombre;
        }
    }
}
