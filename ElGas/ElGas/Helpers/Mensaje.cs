using System;
using System.Collections.Generic;
using System.Text;

namespace ElGas.Helpers
{
    public static class Mensaje
    {

        public static class Contenido
        {

            public static string SinInternet { get { return "No se ha podido completar la solicitud, por favor revise su conexión."; } }
            public static string SinCobertura { get { return "El pedido se encuentra fuera de nuestra cobertura, pronto estaremos en su sector."; } }
            public static string Excepcion { get { return "No se ha podido completar la solicitud, hemos tenido problema al conectarnos al servicio intente de nuevo por favor."; } }
            public static string NoDeterminaPosicion { get { return "No se ha podido determinar la posición actual, es posible que no esté activo el GPS o no tenga conección a internet."; } }
            public static string PedidoAtendidoOtroDistribuidor { get { return "El pedido fue atendido por otro distribuidor"; } }
            public static string PedidoAnuladoCliente { get { return "La solicitud del pedido ha sido anulada por el cliente"; } }

            public static string FueraDelHorario { get { return string.Format("Gracias por su intetes, nuestro horario de atención es de {0} a {1}. \nLe esperamos", TimeSpan.FromHours(Settings.HoraIncial).ToString("h\\:mm")  , TimeSpan.FromHours(Settings.HoraFinal).ToString("h\\:mm")); } }



        }

        public static class TextoBoton
        {
            public static string Aceptar { get { return "Aceptar"; } }
        }

        public static class Titulo
        {
            public static string Informacion { get { return "Información"; } }
            public static string Error { get { return "Error"; } }
        }

    }

   
}
