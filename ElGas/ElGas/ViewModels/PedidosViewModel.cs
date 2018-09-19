using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ElGas.Helpers;
using ElGas.Models;
using ElGas.Pages;
using ElGas.Services;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace ElGas.ViewModels
{
    public class PedidosViewModel : INotifyPropertyChanged
    {
        private Command<object> tapCommand;
        public event PropertyChangedEventHandler PropertyChanged;


        public ObservableCollection<ComprasRequest> listaCompra;
        public ObservableCollection<ComprasRequest> ListaCompra
        {
            protected set
            {
                listaCompra = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ListaCompra"));
            }
            get { return listaCompra; }
        }




        public bool noHayPedidos = false;

        public bool NoHayPedidos
        {
            set
            {
                noHayPedidos = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoHayPedidos"));

            }
            get
            {
                return noHayPedidos;
            }
        }

        // api/ListCompraByClient

        //Actualizar pedidos, cuando se realice una accion
        public PedidosViewModel()
        {
            try
            {
                listaCompra = new ObservableCollection<ComprasRequest>();
                ListaCompra = new ObservableCollection<ComprasRequest>();

                if (CrossConnectivity.Current.IsConnected)
                {
                    MisPedidos();
                    tapCommand = new Command<object>(iralDetalle);
                }
                else
                {
                     App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.SinInternet, Mensaje.TextoBoton.Aceptar);
                }
            }
            catch (Exception ex)
            {

                Debug.Write(ex.Message);
            }
        }


      

        async void MisPedidos()
        {
            /// -1 que se cancelo
            /// 0 que nadie la acepta aún
            /// 1 que ya un repartidor la está atendiendo
            /// 2 que ya se realizo la venta

            try
            {
                ListaCompra = new ObservableCollection<ComprasRequest>();

                var d = new Cliente { IdCliente = Settings.idCliente };
                var response = await ApiServices.InsertarAsync<Cliente>(d, new System.Uri(Constants.BaseApiAddress), "/api/Compras/ListCompraByClient");
                var list = JsonConvert.DeserializeObject<List<ComprasRequest>>(response.Result.ToString());
                if (list.Count == 0)
                {
                    NoHayPedidos = true;
                }
                
                foreach (var item in list.OrderByDescending(o => o.IdCompra).ToList())
                {
                    var fecha = TimeZoneInfo.ConvertTime(item.FechaPedido.Value, TimeZoneInfo.Local);


                    switch (item.Estado)
                    {
                        case -1:
                            item.TituloPedido = "Cancelado";
                            item.icono = "cancelado.png";
                            break;
                        case 0:
                            item.TituloPedido = "No atendido";
                            item.icono = "pendiente.png";
                            break;
                        case 1:
                            item.TituloPedido = "Pendiente";
                            item.icono = "aceptado.png";
                            break;
                        case 2:
                            item.TituloPedido = "Exitoso";
                            item.icono = "vendido.png";
                            break;
                    }


                    //item.TituloPedido+= " "+fecha.ToString("yyyy-MM-dd"); 
                    item.FechaTexto = fecha.ToString("yyyy-MMM-dd   hh:mm");
                    ListaCompra.Add(item);

                }
            }
            catch (Exception ex)
            {

                Debug.Write(ex.Message);
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
            await App.Navigator.PushAsync(new DetallePage(compra), true);
            Debug.WriteLine(compra.IdCompra);
            
        }

        public ICommand RefreshPedidos { get { return new RelayCommand(MisPedidos); } }

    }
}
