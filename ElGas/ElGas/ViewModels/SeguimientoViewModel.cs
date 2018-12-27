using ElGas.Pages;
using ElGas.Services;
using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using Plugin.Messaging;
using ElGas.Helpers;
using System.Collections.ObjectModel;
using TK.CustomMap;
using ElGas.Models;
using Xamarin.Forms;
using Newtonsoft.Json;
using Firebase.Xamarin.Database;
using Plugin.Connectivity;
using Firebase.Xamarin.Database.Streaming;
using System.Linq;

namespace ElGas.ViewModels
{
    public class SeguimientoViewModel: INotifyPropertyChanged
    {
        #region services
        ApiServices apiService = new ApiServices();
        #endregion
        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;
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
        public Distribuidor distribuidor;
        public Distribuidor Distribuidor {
            get { return distribuidor; }
            set
            {
                if (this.distribuidor != value)
                {

                    this.distribuidor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Distribuidor"));
                }
            }
        }

        public ObservableCollection<DistribuidorFirebase> camiones;
        public ObservableCollection<DistribuidorFirebase> Camiones
        {
            protected set
            {
                camiones = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Camiones"));
            }
            get { return camiones; }
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


        private readonly string ElGAS_FIREBASE = "https://elgas-f24e8.firebaseio.com/-LJVkHULelfySFjNF9-Q/Equipo-ElGas/";
        private readonly FirebaseClient _firebaseClient;


        #endregion



        #region Constructor
        public SeguimientoViewModel()
        {
            try
            {
                distribuidor = new Distribuidor();
                camiones = new ObservableCollection<DistribuidorFirebase>();
                Camiones = new ObservableCollection<DistribuidorFirebase>();
                Locations = new ObservableCollection<TKCustomMapPin>();
                locations = new ObservableCollection<TKCustomMapPin>();
                centerSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(-0.180653, -78.46783820000002)), Distance.FromMiles(2)));

                if (CrossConnectivity.Current.IsConnected)
                {
                    _firebaseClient = new FirebaseClient(ElGAS_FIREBASE);
                    //Device.BeginInvokeOnMainThread(async () =>
                    //{
                    //    await loadParametros();
                    //});
                    DatosVendedor();
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);                
            }
        }
        #endregion
        #region Commands
        public ICommand ContactCommand { get { return new RelayCommand(Contact); } }
        public async void Contact()
        {
            try
            {
                var PhoneCallTask = CrossMessaging.Current.PhoneDialer;
                if (PhoneCallTask.CanMakePhoneCall)
                {
                    PhoneCallTask.MakePhoneCall(distribuidor.Telefono, distribuidor.Nombres);
                }
            }
            catch (Exception ex)
            {

                Debug.Write(ex.Message);
            }
        }

        public ICommand CancelCommand { get { return new RelayCommand(Cancel); } }
        public async void Cancel()
        {

            var action = await App.Current.MainPage.DisplayAlert("Pedido a Cancelar",string.Format( "{0} Tanque(s)",Settings.TanquesGas), "Confirmar", "Regresar");
            if (action)
            {
                var compra = new Compra { IdCompra = Settings.IdCompra, IdDistribuidor=Settings.IdDistribuidor};
                var compracancelada = new CompraCancelada
                {
                    IdCompra = Settings.IdCompra,
                    IdDistribuidor = Settings.IdDistribuidor,
                    CanceladaPor = 1,
                    IdCliente = Settings.idCliente
                };
                var response = await ApiServices.InsertarAsync<CompraCancelada>(compracancelada, new Uri(Constants.BaseApiAddress), "/api/Compras/Cancelar");
                if(response.IsSuccess)
                {
                    await App.Current.MainPage.DisplayAlert("Pedido a Cancelar", string.Format("Su pedido de {0} tanque(s) ha sido cancelado", Settings.TanquesGas), "Aceptar");
                    Settings.Pedidos = false;
                    await App.Navigator.Navigation.PopToRootAsync();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Problemas", string.Format("Tenemos problemas para cancelarsu pedido de {0} tanque(s), trabajamos para solucionarlo", Settings.TanquesGas), "Aceptar");
                    //Settear las variables globales
                    Settings.Pedidos = false;
                    Settings.IdCompra = 0;
                    Settings.IdDistribuidor = 0;
                    Settings.TanquesGas = 0;
                    Settings.Pedidos = false;
                    await App.Navigator.Navigation.PopToRootAsync();
                }              
            }
        }
        public async void DatosVendedor()
        {
            var d = new Distribuidor { IdDistribuidor=Settings.IdDistribuidor };
            var response = await ApiServices.InsertarAsync<Distribuidor>(d, new System.Uri(Constants.BaseApiAddress), "/api/Distribuidors/GetDistribuidorID");
            Distribuidor = JsonConvert.DeserializeObject<Distribuidor>(response.Result.ToString());

            Distribuidor.Nombres = string.Format("{0} {1}","Su pedido fue atendido por:",Distribuidor.Nombres);
            Distribuidor.PlacaVehiculo = string.Format("{0} {1}", "Placa del vehículo:", Distribuidor.PlacaVehiculo);
            #region Forma Firebase

            Locations.Clear();
            // Point p = new Point(0.48, 0.96);

            _firebaseClient
                .Child("Distribuidores")

            .AsObservable<DistribuidorFirebase>()
            .Subscribe(evento =>
            {
                if (evento.EventType == FirebaseEventType.InsertOrUpdate)
                {
                    Device.BeginInvokeOnMainThread(() => {
                        AdicionarPedido(evento.Key, evento.Object,Distribuidor.FirebaseID);
                    });

                }
                if (evento.EventType == FirebaseEventType.Delete)
                {
                        //accion para borrar
                }
            });

           // SeguirA();

            #endregion


        }

        private void AdicionarPedido(string key, DistribuidorFirebase pedido, string idfirebase)
        {
            // Locations.Clear();



            if (key==idfirebase)
            {
                Point p = new Point(0.48, 0.96);
                var found = Camiones.FirstOrDefault(x => x.id == pedido.id);
                if (found != null)
                {
                    int i = Camiones.IndexOf(found);
                    Camiones[i] = pedido;

                    int y = Locations.IndexOf(Locations.FirstOrDefault(x => x.ID == pedido.id.ToString()));

                    Locations.RemoveAt(y);
                    var Pindistribuidor = new TKCustomMapPin
                    {
                        Image = "camion",
                        Position = new TK.CustomMap.Position((double)pedido.Latitud, (double)pedido.Longitud),
                        Anchor = p,
                        ShowCallout = true,
                        ID = pedido.id.ToString()
                    };
                    Locations.Add(Pindistribuidor);
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Locations"));

                }
                else
                {

                    Camiones.Add(new DistribuidorFirebase()
                    {
                        id = pedido.id,
                        Latitud = pedido.Latitud,
                        Longitud = pedido.Longitud,
                    });
                    var Pindistribuidor = new TKCustomMapPin
                    {
                        Image = "camion",
                        Position = new TK.CustomMap.Position((double)pedido.Latitud, (double)pedido.Longitud),
                        Anchor = p,
                        ShowCallout = true,
                        ID = pedido.id.ToString()
                    };
                    Locations.Add(Pindistribuidor);

                } 
            }
        }

        public async void SeguirA()
        {
            if (Settings.IdDistribuidor != 0)
            {
                var distribuidor = new Distribuidor
                {
                    IdDistribuidor = Settings.IdDistribuidor,

                };
                Point p = new Point(0.48, 0.96);


                var response = await ApiServices.InsertarAsync<Distribuidor>(distribuidor, new Uri(Constants.BaseApiAddress), "/api/Rutas/GetLastPosition");
                var ruta = JsonConvert.DeserializeObject<Ruta>(response.Result.ToString());

                Locations.Clear();

                Locations.Add(new TKCustomMapPin
                {
                    Image = "camion.png",
                    Position = new TK.CustomMap.Position((double)ruta.Latitud, (double)ruta.Longitud),
                    Anchor = p,
                    ShowCallout = true,

                });
                CenterSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position((double)ruta.Latitud, (double)ruta.Longitud)), Distance.FromMiles(.5)));
            }
        }
           
        #endregion
        }
}
