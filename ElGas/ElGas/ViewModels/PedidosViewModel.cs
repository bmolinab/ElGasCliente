using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using ElGas.Helpers;
using ElGas.Models;
using ElGas.Pages;
using ElGas.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ElGas.ViewModels
{
    public class PedidosViewModel
    {
        private Command<object> tapCommand;

        public ObservableCollection<ComprasRequest> ListaCompra { get; set; }
        // api/ListCompraByClient
        public PedidosViewModel()
        {
            MisPedidos();
            tapCommand = new Command<object>(iralDetalle);
        }

        async void MisPedidos()
        {
            /// -1 que se cancelo
            /// 0 que nadie la acepta aún
            /// 1 que ya un repartidor la está atendiendo
            /// 2 que ya se realizo la venta

            ListaCompra = new ObservableCollection<ComprasRequest>();

            var d = new Cliente { IdCliente = Settings.idCliente };
            var response = await ApiServices.InsertarAsync<Cliente>(d, new System.Uri(Constants.BaseApiAddress), "/api/Compras/ListCompraByClient");
            var  list = JsonConvert.DeserializeObject<List<ComprasRequest>>(response.Result.ToString());

            foreach (var item in list)
            {
                var fecha = TimeZoneInfo.ConvertTime(item.FechaPedido.Value.Date, TimeZoneInfo.Local);

          
                switch (item.Estado)
                {
                    case -1:
                        item.TituloPedido = "Compra Cancelada";
                        item.icono = "cancelado.png";
                        break;
                    case 0:
                        item.TituloPedido = "Compra no atendida";
                        item.icono = "pendiente.png";
                        break;
                    case 1:
                        item.TituloPedido = "Compra en atención";
                        item.icono = "aceptado.png";
                        break;
                    case 2:
                        item.TituloPedido = "Compra Finalizada";
                        item.icono = "vendido.png";
                        break;
                }


                item.TituloPedido+= " "+fecha.ToString("yyyy-MM-dd");                
                ListaCompra.Add(item);

            }
        }

        public Command<object> TapCommand
        {
            get { return tapCommand; }
            set { tapCommand = value;  }
        }

        private async void iralDetalle(object obj)
        {
            var compra = (ComprasRequest)obj;


            await App.Navigator.PushAsync(new  Page1());
             //await App.Navigator.PushAsync(new Page1());

            Debug.WriteLine(compra.IdCompra);
            
        }
    }
}
