using ElGas.Helpers;
using ElGas.Services;
using GalaSoft.MvvmLight.Views;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
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

        #endregion

        bool tracking;
        public MapaViewModel()
        {

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

                CenterSearch = (MapSpan.FromCenterAndRadius((new TK.CustomMap.Position(position.Latitude,position.Longitude)), Distance.FromMiles(.3)));


                var Distribuidores = await apiService.DistribuidoresCercanos(new Models.Posicion {Latitud=position.Latitude, Longitud= position.Longitude });

                Debug.WriteLine(Distribuidores.Count);


            }
                catch (Exception ex)
                {
                Debug.WriteLine("Uh oh", "Something went wrong, but don't worry we captured for analysis! Thanks.", "OK");
                }            
            }

    }
}
