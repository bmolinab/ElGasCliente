using System;
namespace ElGas.Models
{
    public class Ciudad
    {

        public string Id
        {
            get;
            set;
        }

        public string Nombre
        {
            get;
            set;
        }

        public Ciudad(string id, string nombre)
        {
            this.Id = id;
            this.Nombre = nombre;
        }

        public override string ToString()
        {
            return this.Nombre;
        }
        public Ciudad()
        {

        }
    }
}
