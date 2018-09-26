

using System.Diagnostics;
using ElGas.iOS.Models;
using Foundation;
using Newtonsoft.Json;
using Plugin.DeviceInfo;
using Plugin.FacebookClient;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.SfRating.XForms.iOS;
using TK.CustomMap.iOSUnified;
using UIKit;
using UserNotifications;
using WindowsAzure.Messaging;



namespace ElGas.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private SBNotificationHub Hub { get; set; }
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();
            SfListViewRenderer.Init();

            var render= new SfRatingRenderer();

            Rg.Plugins.Popup.Popup.Init();
         //  EnhancedEntryRenderer.Init();
            var renderer = new TKCustomMapRenderer();          

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Sound,
                                                                        (granted, error) =>
                                                                        {
                                                                            if (granted)
                                                                                InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                                                                        });
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                        UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                        new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }

            LoadApplication(new App());
            FacebookClientManager.Initialize(app, options);

            return base.FinishedLaunching(app, options);


        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Hub = new SBNotificationHub(Constants.ListenConnectionString, Constants.NotificationHubName);

            Hub.UnregisterAllAsync(deviceToken, (error) => {
                if (error != null)
                {
                    System.Diagnostics.Debug.WriteLine("Error calling Unregister: {0}", error.ToString());
                    return;
                }
                Helpers.Settings.DeviceID = CrossDeviceInfo.Current.Id;

                NSSet tags = new NSSet(Helpers.Settings.DeviceID,"Cliente")  ;

                // create tags if you want
                Hub.RegisterNativeAsync(deviceToken, tags, (errorCallback) => {
                    if (errorCallback != null)
                        System.Diagnostics.Debug.WriteLine("RegisterNativeAsync error: " + errorCallback.ToString());
                });
            });
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            ProcessNotification(userInfo, false);
        }

        void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
        {
            // Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
            if (null != options && options.ContainsKey(new NSString("aps")))
            {
                //Get the aps dictionary
                NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

                NSDictionary result = options.ObjectForKey(new NSString("Data")) as NSDictionary;

                var data = new DataRequest();
                data.idCompra=(result[new NSString("idCompra")] as NSString).ToString();
                data.tipo = (result[new NSString("tipo")] as NSString).ToString();
                data.idDistribuidor = (result[new NSString("idDistribuidor")] as NSString).ToString();

                switch (data.tipo)
                {
                    case "1":
                        Helpers.Settings.Pedidos = true;
                        if (data.idDistribuidor != null && data.idDistribuidor != "")
                        {
                            Helpers.Settings.IdDistribuidor = int.Parse(data.idDistribuidor);
                        }

                        Helpers.Settings.IdCompra = int.Parse(data.idCompra);


                        break;
                    case "3":
                        Helpers.Settings.Pedidos = false;
                        Helpers.Settings.Calificar = true;

                        break;
                    case "5":
                        Helpers.Settings.Pedidos = false;
                        Helpers.Settings.IdDistribuidor = new int();

                        break;
                }



                string alert = string.Empty;



                //Extract the alert text
                // NOTE: If you're using the simple alert by just specifying
                // "  aps:{alert:"alert msg here"}  ", this will work fine.
                // But if you're using a complex alert with Localization keys, etc.,
                // your "alert" object from the aps dictionary will be another NSDictionary.
                // Basically the JSON gets dumped right into a NSDictionary,
                // so keep that in mind.
                if (aps.ContainsKey(new NSString("alert")))
                    alert = (aps[new NSString("alert")] as NSString).ToString();



               




                //If this came from the ReceivedRemoteNotification while the app was running,
                // we of course need to manually process things like the sound, badge, and alert.
                if (!fromFinishedLaunching)
                {
                    //Manually show an alert
                    if (!string.IsNullOrEmpty(alert))
                    {
                        UIAlertView avAlert = new UIAlertView("El Gas", alert, null, "OK", null);
                        avAlert.Show();

                        Xamarin.Forms.MessagingCenter.Send<string>("update", "Hi");

                    }
                }
            }
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);
            FacebookClientManager.OnActivated();
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return FacebookClientManager.OpenUrl(app, url, options);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return FacebookClientManager.OpenUrl(application, url, sourceApplication, annotation);
        }

    }
}
