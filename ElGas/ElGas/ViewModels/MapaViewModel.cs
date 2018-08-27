using ElGas.Helpers;
using ElGas.Models;
using ElGas.Pages;
using ElGas.Services;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Streaming;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TK.CustomMap;
using Xamarin.Forms;

namespace ElGas.ViewModels
{
    public class MapaViewModel : INotifyPropertyChanged
    {

        #region services
        ApiServices apiService = new ApiServices();

        #endregion
        #region Properties
        private bool _isVisible = false;
        public bool isVisible
        {
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("isVisible"));
                }
            }
            get
            {
                return _isVisible;
            }
        }

        private bool oneButton = true;
        public bool OneButton
        {
            set
            {
                if (oneButton != value)
                {
                    oneButton = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OneButton"));
                }
            }
            get
            {
                return oneButton;
            }
        }
        bool tracking;


        private readonly string ElGAS_FIREBASE = "https://elgas-f24e8.firebaseio.com/-LJVkHULelfySFjNF9-Q/Equipo-ElGas/";
        private readonly FirebaseClient _firebaseClient;  

        #endregion
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
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

        //private ObservableCollection<DistribuidorFirebase> camiones=new ObservableCollection<DistribuidorFirebase>();

        //public ObservableCollection<DistribuidorFirebase> Camiones
        //{
        //    get { return camiones; }
        //    set
        //    {
        //        camiones = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Camiones"));
        //    }
        //}


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

        public ObservableCollection<TKCustomMapPin> locations;
        public ObservableCollection<TKCustomMapPin> Locations
        {
            protected set
            {
                locations = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Locations"));
            }
            get { return locations; }
        }

        #endregion
        Xamarin.Forms.Maps.Geocoder geoCoder;
        public string direccion = "";
        public string Direccion
        {
            get { return direccion; }
            set { direccion = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Direccion")); }

        }
        #region Constructor
        public MapaViewModel()
        {
            _firebaseClient = new FirebaseClient(ElGAS_FIREBASE);
            camiones = new ObservableCollection<DistribuidorFirebase>();
            Camiones = new ObservableCollection<DistribuidorFirebase>();
            geoCoder = new Xamarin.Forms.Maps.Geocoder();
            Locations = new ObservableCollection<TKCustomMapPin>();
            locations = new ObservableCollection<TKCustomMapPin>();
            centerSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(0, 0)), Distance.FromMiles(.3)));
            Device.BeginInvokeOnMainThread(async () =>
            {
                await loadParametros();
            }); 
            LoadVendedores();
        }
        #endregion

        public void OnAppearing()
        {
            Locations = new ObservableCollection<TKCustomMapPin>();
            locations = new ObservableCollection<TKCustomMapPin>();
            centerSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(0, 0)), Distance.FromMiles(.3)));
            LoadVendedores();
            //Do whatever you like in here
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

        public async Task<bool> loadParametros()
        {
            Cliente cliente = new Cliente
            {
                IdCliente = Settings.idCliente,
            };

            var response = await ApiServices.InsertarAsync<Cliente>(cliente, new Uri(Constants.BaseApiAddress), "/api/Parametroes/GetAllParameters");
            var parametros = JsonConvert.DeserializeObject<List<Parametro>>(response.Result.ToString());
            if (parametros!=null)

            {
                bool Actualizado = true;
                foreach (var item in parametros)
                {
                    if (item.Nombre== "valor")
                    {
                        Settings.Precio =(double) item.Valor;
                    }
                    if(item.Nombre== "versioncliente")
                    {
                        if(Constants.VersionCliente >= item.Valor)
                        {
                            Actualizado = true;
                        }
                        else
                        {
                            Actualizado = false;
                        }
                    }
                }
                if(!Actualizado) await App.Navigator.PushAsync(new UpdatePage());


            }
            return true;



        }
        public async void LoadVendedores()
        {        
                try
                {
                //     await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        Debug.WriteLine("Need location", "Gunna need that location", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {
                    //Permission granted, do what you want do.
                }
                else if (status != PermissionStatus.Unknown)
                {
                    Debug.WriteLine("Location Denied", "Can not continue, try again.", "OK");
                }





                var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 10;//DesiredAccuracy.Value;
                    Debug.WriteLine("Consiguiendo localización...");
                    var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(3), null, true);
                    if (position == null)
                    {
                        Debug.WriteLine("null gps :(");
                        return;
                    }
                    CenterSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(position.Latitude, position.Longitude)), Distance.FromMiles(.5)));
                #region Forma Antigua

                //var Distribuidores = await apiService.DistribuidoresCercanos(new Models.Posicion { Latitud = position.Latitude, Longitud = position.Longitude });
                //    Locations.Clear();
                //    Point p = new Point(0.48, 0.96);

                //        foreach (var distribuidor in Distribuidores)
                //        {
                //            var Pindistribuidor = new TKCustomMapPin
                //            {
                //                Image = "camion",
                //                Position = new TK.CustomMap.Position((double)distribuidor.Latitud, (double)distribuidor.Longitud),
                //                Anchor = p,
                //                ShowCallout = true,
                //            };
                //            Debug.WriteLine(Pindistribuidor.Image);
                //            Locations.Add(Pindistribuidor);
                //        }
                //        Debug.WriteLine(Distribuidores.Count);
                #endregion

                #region Forma Firebase

                    Locations.Clear();
                   // Point p = new Point(0.48, 0.96);

                _firebaseClient
                .Child("Distribuidores")
                .AsObservable<DistribuidorFirebase>()
                .Subscribe(d =>
                {
                    if (d.EventType == FirebaseEventType.InsertOrUpdate)
                    {
                        Device.BeginInvokeOnMainThread(() => {
                            AdicionarPedido(d.Key, d.Object);
                        });

                    }
                    if (d.EventType == FirebaseEventType.Delete)
                    {
                        //accion para borrar
                    }
                });
                #endregion
            }
            catch (Exception ex)
             {
                Debug.WriteLine("Uh oh", "Algo salió mal, ¡pero no te preocupes capturamos para el análisis! Gracias.", "OK");
             }            
        }
        #region Tareas

        private void AdicionarPedido(string key, DistribuidorFirebase pedido)
        {
           // Locations.Clear();

           if (!isVisible)
            {

            
            Point p = new Point(0.48, 0.96);
            var found = Camiones.FirstOrDefault(x => x.id == pedido.id);
            if (found != null)
            {
                int i = Camiones.IndexOf(found);
                Camiones[i] = pedido;

                int y = Locations.IndexOf( Locations.FirstOrDefault(x => x.ID == pedido.id.ToString()));

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
        #endregion

        #region commands
        public ICommand BuyCommand { get { return new RelayCommand(Buy); } }
        private async void Buy()
        {

            var lat = CenterSearch.Center.Latitude;
            var lon = CenterSearch.Center.Longitude;
             CenterSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(lat, lon)), Distance.FromMiles(.10)));

            Locations.Clear();
            Camiones.Clear();

            Locations.Add(new TKCustomMapPin
            {
                Image = "casa",
                Position = CenterSearch.Center,
                Anchor = new Point(0.48, 0.96),
                ShowCallout = true,
            });

           
            


            ObtenerDireccion(CenterSearch.Center.Latitude, CenterSearch.Center.Longitude);

            isVisible = true;
            OneButton = false;
            


        }
        public ICommand CancelCommand { get { return new RelayCommand(Cancel); } }
        private async void Cancel()
        {
            LoadVendedores();
            isVisible = false;
            OneButton = true;

        }

        public ICommand OkCommand { get { return new RelayCommand(Ok); } }
        private async void Ok()
        {
            if(Locations.Count>0)
            {
                isVisible = false;
                OneButton = true;
                var ubicacion =Locations[0];
                Settings.Direccion = Direccion;
                Debug.WriteLine("Latitud:{0} Longitud:{1}",ubicacion.Position.Latitude, ubicacion.Position.Longitude);
                await App.Navigator.PushAsync(new Confirmacion(ubicacion));
            }
        }
        public Command<TK.CustomMap.Position> MapClickedCommand
        {
            get
            {                
                return new Command<TK.CustomMap.Position>((positon) =>
                {
                    //Determine if a point was inside a circle
                    if (isVisible)
                    {                        
                        Locations.Clear();               
                        
                        Locations.Add(new TKCustomMapPin {
                            Image = "casa",
                            Position = positon,
                            Anchor = new Point(0.48, 0.96),
                            ShowCallout = true, });

                        ObtenerDireccion(positon.Latitude, positon.Longitude);

                    }
                });
            }
        }
      
        #endregion
    }
}
