using ElGas.Helpers;
using ElGas.Models;
using ElGas.Pages;
using ElGas.Services;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Streaming;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using Plugin.Connectivity;
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
        Xamarin.Forms.Maps.Geocoder geoCoder;
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

        public string direccion = "";
        public string Direccion
        {
            get { return direccion; }
            set { direccion = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Direccion")); }

        }

        private readonly string ElGAS_FIREBASE = "https://elgas-f24e8.firebaseio.com/-LJVkHULelfySFjNF9-Q/Equipo-ElGas/";
        private readonly FirebaseClient _firebaseClient;


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
                    ObtenerDireccion(value.Center.Latitude, value.Center.Longitude);
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
        #region Constructor
        public MapaViewModel()
        {


            try
            {
                camiones = new ObservableCollection<DistribuidorFirebase>();
                Camiones = new ObservableCollection<DistribuidorFirebase>();
                geoCoder = new Xamarin.Forms.Maps.Geocoder();
                Locations = new ObservableCollection<TKCustomMapPin>();
                locations = new ObservableCollection<TKCustomMapPin>();
                centerSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(-0.180653, -78.46783820000002)), Distance.FromMiles(2)));
                if (CrossConnectivity.Current.IsConnected)
                {
                    _firebaseClient = new FirebaseClient(ElGAS_FIREBASE);

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await loadParametros();
                    });
                    LoadVendedores();
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }
        #endregion
        #region Events
        public void OnAppearing()
        {
            try
            {
                Locations = new ObservableCollection<TKCustomMapPin>();
                locations = new ObservableCollection<TKCustomMapPin>();
                Locations.Clear();
                centerSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(-0.180653, -78.46783820000002)), Distance.FromMiles(2)));
                LoadVendedores();
            }
            catch (Exception ex)
            {

                Debug.Write(ex.Message);
            }
            //Do whatever you like in here
        }

        #endregion
        #region Methods
        async void ObtenerDireccion(double lat, double lon)
        {
            try
            {
                var position = new Xamarin.Forms.Maps.Position(lat, lon);
                var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(position);
                foreach (var address in possibleAddresses)
                {
                    try
                    {
                        string[] auxDireccion = address.Split(Convert.ToChar("\n"));
                        Direccion = auxDireccion[0];
                        break;
                    }
                    catch
                    {
                        Direccion = "Ups, no te localizamos";
                    }


                }
            }
            catch (Exception ex)
            {

                Debug.Write(ex.Message);
            }

        }

        public async Task<bool> loadParametros()
        {
            try
            {
                Cliente cliente = new Cliente
                {
                    IdCliente = Settings.idCliente,
                };

                var response = await ApiServices.InsertarAsync<Cliente>(cliente, new Uri(Constants.BaseApiAddress), "/api/Parametroes/GetAllParameters");
                var parametros = JsonConvert.DeserializeObject<List<Parametro>>(response.Result.ToString());
                if (parametros != null)

                {
                    bool Actualizado = true;
                    foreach (var item in parametros)
                    {
                        if (item.Nombre == "valor")
                        {
                            Settings.Precio = (double)item.Valor;
                        }
                        if (item.Nombre == "versioncliente")
                        {
                            if (Constants.VersionCliente >= item.Valor)
                            {
                                Actualizado = true;
                            }
                            else
                            {
                                Actualizado = false;
                            }
                        }

                        if (item.Nombre == "horainicial")
                        {
                            var valor = (double)item.Valor;
                            Settings.HoraIncial = valor;
                            //TimeSpan timespan = TimeSpan.FromHours(Settings.HoraIncial);
                            //string output = timespan.ToString("h\\:mm");
                            //Debug.WriteLine(output);
                        }

                        if (item.Nombre == "horafinal")
                        {
                            var valor = (double)item.Valor;
                            Settings.HoraFinal = valor;
                            //TimeSpan timespan = TimeSpan.FromHours(valor);
                            //string output = timespan.ToString("h\\:mm");
                            //Debug.WriteLine(output);
                        }

                    }
                    if (!Actualizado) await App.Navigator.PushAsync(new UpdatePage());


                }
            }
            catch (Exception ex)
            {

                Debug.Write(ex.Message);
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
                        Device.BeginInvokeOnMainThread(() =>
                        {
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

        private void AdicionarPedido(string key, DistribuidorFirebase pedido)
        {
            // Locations.Clear();

            try
            {
                if (!isVisible)
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
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }
        }
        #endregion
        #region commands
        public ICommand BuyCommand { get { return new RelayCommand(Buy); } }
        private async void Buy()
        {

            if (CrossConnectivity.Current.IsConnected)
            {
                var locator = CrossGeolocator.Current;

                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(3), null, true);
                var servidor = await apiService.Horario(new SolicitudesFallidas { Latitud = position.Longitude, Longitud = position.Longitude,IdCliente=Settings.idCliente});
                if (servidor.IsSuccess=false && Convert.ToInt32(servidor.Result.ToString())==-2)
                {
                    await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Informacion, "No se ha podido conectar al servicio", Mensaje.TextoBoton.Aceptar);
                    return;
                }

                if (!servidor.IsSuccess && int.Parse(servidor.Result.ToString()) == 0)
                {
                    await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Informacion, Mensaje.Contenido.FueraDelHorario, Mensaje.TextoBoton.Aceptar);
                    return;
                }


                try
                {
                    var d = new Cliente { IdCliente = Settings.idCliente };
                    var response = await ApiServices.InsertarAsync<Cliente>(d, new System.Uri(Constants.BaseApiAddress), "/api/Compras/CompraPendiente");
                    var pedidos = JsonConvert.DeserializeObject<int>(response.Result.ToString());

                    if (pedidos > 0)
                    {
                        bool action = false;

                        if (pedidos > 1)
                        {
                            action = await App.Current.MainPage.DisplayAlert("Aviso", "Usted tiene  " + pedidos + " pedidos pendiente, desea continuar", "Continuar", "Cancelar");
                        }
                        else
                        {
                            action = await App.Current.MainPage.DisplayAlert("Aviso", "Usted tiene  " + pedidos + " pedido pendiente, desea continuar", "Continuar", "Cancelar");
                        }
                        if (!action)
                        {
                            return;
                        }
                    }


                    var lat = CenterSearch.Center.Latitude;
                    var lon = CenterSearch.Center.Longitude;
                    CenterSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(lat, lon)), Distance.FromMiles(.10)));

                    Locations.Clear();
                    Camiones.Clear();

                    //Locations.Add(new TKCustomMapPin
                    //{
                    //    Image = "casa",
                    //    Position = CenterSearch.Center,
                    //    Anchor = new Point(0.48, 0.96),
                    //    ShowCallout = true,
                    //    ID = "casa"
                    //});


                    ObtenerDireccion(CenterSearch.Center.Latitude, CenterSearch.Center.Longitude);
                    isVisible = true;
                    OneButton = false;
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message);
                    throw;
                }

            }
            else
            {
                await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.SinInternet, Mensaje.TextoBoton.Aceptar);
            }


        }
        public ICommand CancelCommand { get { return new RelayCommand(Cancel); } }
        private async void Cancel()
        {
            try
            {
                Locations.Clear();
                LoadVendedores();
                isVisible = false;
                OneButton = true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }

        }

        public ICommand OkCommand { get { return new RelayCommand(Ok); } }
        private async void Ok()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {

                    isVisible = false;
                    OneButton = true;
                    var ubicacion = new TKCustomMapPin
                    {
                        Position = CenterSearch.Center,
                        Image = "casa",
                        Anchor = new Point(0.48, 0.96),
                        ShowCallout = true,
                        ID = "casa"
                    };
                    Settings.Direccion = Direccion;
                    Debug.WriteLine("Latitud:{0} Longitud:{1}", ubicacion.Position.Latitude, ubicacion.Position.Longitude);
                    Locations.Clear();

                    var posicion = new Posicion { Latitud = ubicacion.Position.Latitude, Longitud = ubicacion.Position.Longitude };

                    var response = await ApiServices.InsertarAsync<Posicion>(posicion, new Uri(Constants.BaseApiAddress), "/api/Cobertura/TieneCobertura");
                    if (response.IsSuccess)
                    {
                        await App.Navigator.PushAsync(new Confirmacion(ubicacion));
                        return;
                    }
                    else if (int.Parse(response.Result.ToString()) == 1)
                    {
                        await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Informacion, Mensaje.Contenido.SinCobertura, Mensaje.TextoBoton.Aceptar);
                        return;
                    }





                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message);
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.SinInternet, Mensaje.TextoBoton.Aceptar);
            }
        }
        public Command<TK.CustomMap.Position> MapClickedCommand
        {
            get
            {
                return new Command<TK.CustomMap.Position>((positon) =>
                {
                    //Determine if a point was inside a circle
                    try
                    {
                        if (isVisible)
                        {
                            Locations.Clear();

                            Locations.Add(new TKCustomMapPin
                            {
                                Image = "casa",
                                Position = positon,
                                Anchor = new Point(0.48, 0.96),
                                ShowCallout = true,
                            });

                            ObtenerDireccion(positon.Latitude, positon.Longitude);

                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.Message);
                    }
                });
            }
        }


        #endregion
    }
}
