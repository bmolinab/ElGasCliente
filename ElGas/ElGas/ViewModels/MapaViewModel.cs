using ElGas.Helpers;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace ElGas.ViewModels
{
    public class MapaViewModel
    {
        bool tracking;
        public MapaViewModel()
        {

        }

        public async void LoadClientes()
        {
            try
            {
                var hasPermission = await Utils.CheckPermissions(Permission.Location);
                if (!hasPermission)
                    return;

                if (tracking)
                {
                    CrossGeolocator.Current.PositionChanged -= CrossGeolocator_Current_PositionChanged;
                    CrossGeolocator.Current.PositionError -= CrossGeolocator_Current_PositionError;
                }
                else
                {
                    CrossGeolocator.Current.PositionChanged += CrossGeolocator_Current_PositionChanged;
                    CrossGeolocator.Current.PositionError += CrossGeolocator_Current_PositionError;
                }

                if (CrossGeolocator.Current.IsListening)
                {
                    await CrossGeolocator.Current.StopListeningAsync();
                    //labelGPSTrack.Text = "Stopped tracking";
                    //ButtonTrack.Text = "Start Tracking";
                    tracking = false;
                    //count = 0;
                }
                else
                {
                   // Positions.Clear();
                    if (await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(30), 1,
                        false, new ListenerSettings
                        {
                        
                        }))
                    {
                        //labelGPSTrack.Text = "Started tracking";
                        //ButtonTrack.Text = "Stop Tracking";
                        tracking = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Uh oh, Something went wrong, but don't worry we captured for analysis! Thanks.");
            }
        }

        void CrossGeolocator_Current_PositionError(object sender, PositionErrorEventArgs e)
        {

            Debug.WriteLine("Location error: " + e.Error.ToString());
        }

        void CrossGeolocator_Current_PositionChanged(object sender, PositionEventArgs e)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
              
            });
        }
    }
}
