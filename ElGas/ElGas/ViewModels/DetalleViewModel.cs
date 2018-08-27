using ElGas.Helpers;
using ElGas.Models;
using ElGas.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using TK.CustomMap;
using Xamarin.Forms;

namespace ElGas.ViewModels
{
    class DetalleViewModel : INotifyPropertyChanged
    {
        #region services
        ApiServices apiService = new ApiServices();
        #endregion
        Xamarin.Forms.Maps.Geocoder geoCoder;

        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;


        public ComprasRequest Compra { get; set; } = new ComprasRequest();
     

        string direccion = "";
        public String Direccion
        {
            get { return direccion; }
            set
            {
                direccion = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Direccion"));
            }
        }

        string fechaCompra = "";
        public String FechaCompra
        {
            get { return fechaCompra; }
            set
            {
                fechaCompra = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FechaCompra"));
            }
        }


        string estadoPedido = "";
        public String EstadoPedido
        {
            get { return estadoPedido; }
            set
            {
                estadoPedido = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EstadoPedido"));
            }
        }

        string cantidadCompra = "";
        public String CantidadCompra
        {
            get { return cantidadCompra; }
            set
            {
                cantidadCompra = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CantidadCompra"));
            }
        }

        string valorCompra = "";
        public String ValorCompra
        {
            get { return valorCompra; }
            set
            {
                valorCompra = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ValorCompra"));
            }
        }

        private bool _isCancelable = false;
        public bool isCancelable
        {
            set
            {
                if (_isCancelable != value)
                {
                    _isCancelable = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("isCancelable"));
                }
            }
            get
            {
                return _isCancelable;
            }
        }

        public ObservableCollection<TKCustomMapPin> locations;
        public ObservableCollection<TKCustomMapPin> Locations
        {
            protected set
            {
                locations = Locations;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Locations"));
            }
            get { return locations; }
        }
        
        public MapSpan centerSearch = null;
        public MapSpan CenterSearch
        {
            get { return centerSearch; }
            set
            {
                if (this.centerSearch != value)
                {

                    this.centerSearch = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CenterSearch"));
                }
            }
        }

        #endregion
        public DetalleViewModel(ComprasRequest comprasRequest)
        {
            try
            {
                EstadoPedido = "";
                Compra = comprasRequest;
                geoCoder = new Xamarin.Forms.Maps.Geocoder();

                Point p = new Point(0.48, 0.96);


                Locations = new ObservableCollection<TKCustomMapPin>();
                locations = new ObservableCollection<TKCustomMapPin>();

                Locations.Clear();

                centerSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position((double)Compra.Latitud, (double)Compra.Longitud)), Distance.FromMiles(.5)));
                Locations.Add(new TKCustomMapPin
                {
                    Image = "casa",
                    Position = new TK.CustomMap.Position((double)Compra.Latitud, (double)Compra.Longitud),
                    Anchor = p,
                    ShowCallout = true,

                });

                switch (comprasRequest.Estado)
                {
                    case -1:
                        EstadoPedido = "Compra Cancelada";
                        isCancelable = false;
                        break;
                    case 0:
                        EstadoPedido = "Compra no atendida";
                        isCancelable = true;
                        break;
                    case 1:
                        EstadoPedido = "Compra en atención";
                        isCancelable = true;
                        break;
                    case 2:
                        EstadoPedido = "Compra Finalizada";
                        isCancelable = false;
                        break;

                }

                var fecha = TimeZoneInfo.ConvertTime(comprasRequest.FechaPedido.Value.Date, TimeZoneInfo.Local);

                FechaCompra= fecha.ToString("yyyy-MM-dd");
                CantidadCompra = Compra.Cantidad.ToString();
                ValorCompra = Compra.ValorTotal.ToString();


                ObtenerDireccion((double)Compra.Latitud, (double)Compra.Longitud);
                


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
           
        }

        async void ObtenerDireccion(double lat, double lon)
        {
            var position = new Xamarin.Forms.Maps.Position(lat, lon);
            var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(position);

            foreach (var address in possibleAddresses)
            {
                Direccion = address;
                break;
            }
        }

        public ICommand CancelCommand { get { return new RelayCommand(Cancel); } }
        public async void Cancel()
        {

            var action = await App.Current.MainPage.DisplayAlert("Pedido a Cancelar", string.Format("{0} Tanque(s)", Compra.Cantidad), "Confirmar", "Regresar");
            if (action)
            {
                var compracancelada = new CompraCancelada
                {
                    IdCompra = Compra.IdCompra,
                    IdDistribuidor = (int)Compra.IdDistribuidor,
                    CanceladaPor = 1,
                    IdCliente =(int) Compra.IdCliente
                };
                var response = await ApiServices.InsertarAsync<CompraCancelada>(compracancelada, new Uri(Constants.BaseApiAddress), "/api/Compras/Cancelar");
                if (response.IsSuccess)
                {
                    await App.Current.MainPage.DisplayAlert("Pedido a Cancelar", string.Format("Su pedido de {0} tanque(s) ha sido cancelado", Compra.Cantidad), "Aceptar");
                    Settings.Pedidos = false;
                    await App.Navigator.Navigation.PopToRootAsync();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Problemas", string.Format("Tenemos problemas para cancelarsu pedido de {0} tanque(s), trabajamos para solucionarlo", Compra.Cantidad), "Aceptar");
                    //Settear las variables globales
                    if(Settings.IdCompra==Compra.IdCompra)
                    {
                        Settings.Pedidos = false;
                        Settings.IdCompra = 0;
                        Settings.IdDistribuidor = 0;
                        Settings.TanquesGas = 0;
                        Settings.Pedidos = false;
                    }
                   

                    await App.Navigator.Navigation.PopToRootAsync();

                }



            }

        }
    }
}
