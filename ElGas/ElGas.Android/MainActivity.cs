using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using TK.CustomMap.Droid;
using Plugin.Permissions;
using WindowsAzure.Messaging;
using Android.Util;
using Android;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Plugin.FacebookClient;
using Android.Content;
using Plugin.Connectivity;

namespace ElGas.Droid
{
    [Activity(Label = "El Gas", Icon = "@drawable/iclauncher", Theme = "@style/MyTheme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const string TAG = "MainActivity";
        const string TAG2 = "MyFirebaseIIDService";
        NotificationHub hub;
  

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;
                Rg.Plugins.Popup.Popup.Init(this, bundle);
                base.OnCreate(bundle);
                Rg.Plugins.Popup.Popup.Init(this, bundle);
                if (Intent.Extras != null)
                {
                    foreach (var key in Intent.Extras.KeySet())
                    {
                        if (key != null)
                        {
                            var value = Intent.Extras.GetString(key);
                            Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                        }
                    }
                }
                FacebookClientManager.Initialize(this);
                global::Xamarin.Forms.Forms.Init(this, bundle);
                Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
                Xamarin.FormsMaps.Init(this, bundle);
                TKGoogleMaps.Init(this, bundle);
                LoadApplication(new App());
            }
            catch (Exception ex )
            {
                var a = ex.Message;
                throw;
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation }, 0);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Permission Granted!!!");
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);
            FacebookClientManager.OnActivityResult(requestCode, resultCode, intent);
        }
    }
}

