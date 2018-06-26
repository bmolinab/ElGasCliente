using ElGas.Helpers;
using ElGas.Pages;
using ElGas.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
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

        #endregion

        bool tracking;
        public MapaViewModel()
        {
            Locations = new ObservableCollection<TKCustomMapPin>();
            locations = new ObservableCollection<TKCustomMapPin>();

            centerSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(0, 0)), Distance.FromMiles(.3)));

            LoadVendedores();
            
        }
        public async void LoadVendedores()
        {        
                try
                {
                    var hasPermission = await Utils.CheckPermissions(Permission.Location);
                    if (!hasPermission)
                        return;
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 10;//DesiredAccuracy.Value;
                    Debug.WriteLine( "Getting gps...");
                    var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(3), null, true);
                    if (position == null)
                    {
                    Debug.WriteLine("null gps :(");
                        return;
                    }
                CenterSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(position.Latitude,position.Longitude)), Distance.FromMiles(.5)));
                var Distribuidores = await apiService.DistribuidoresCercanos(new Models.Posicion {Latitud=position.Latitude, Longitud= position.Longitude });
                Locations.Clear();
                Point p = new Point(0.48, 0.96);
                foreach (var distribuidor in Distribuidores)
                {
                    var Pindistribuidor = new TKCustomMapPin
                    {
                        Image = "camion01",
                        Position = new TK.CustomMap.Position((double)distribuidor.Latitud,(double) distribuidor.Longitud),
                        Anchor = p,
                        ShowCallout = true,
                    };                   
                    Locations.Add(Pindistribuidor);
                }
                Debug.WriteLine(Distribuidores.Count);
            }
            catch (Exception ex)
             {
                Debug.WriteLine("Uh oh", "Something went wrong, but don't worry we captured for analysis! Thanks.", "OK");
             }            
            }
        #region commands
        public ICommand BuyCommand { get { return new RelayCommand(Buy); } }
        private async void Buy()
        {
            isVisible = true;
        }
        public ICommand CancelCommand { get { return new RelayCommand(Cancel); } }
        private async void Cancel()
        {
            isVisible = false;
        }

        public ICommand OkCommand { get { return new RelayCommand(Ok); } }
        private async void Ok()
        {
            if(Locations.Count>0)
            {
                var ubicacion=Locations[0];
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
                            Image = "cliente01",
                            Position = positon,
                            Anchor = new Point(0.48, 0.96),
                            ShowCallout = true, });
                    }
                });
            }
        }
        #endregion
    }
}
