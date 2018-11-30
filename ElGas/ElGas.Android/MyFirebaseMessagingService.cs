using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


using Android.Util;
using Firebase.Messaging;
using Android.Media;
using Android.Graphics;
using Android.Support.V4.App;

namespace ElGas.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";

        private string CHANNEL_ID = "el_gas_cliente_channel_01";
        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);

            if (message.GetNotification() != null)
            {
                //These is how most messages will be received
                Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
                SendNotification(message.GetNotification().Body);
            }
            else
            {
                //Only used for debugging payloads sent from the Azure portal
                string msg = message.Data["message"];
                string tipo = message.Data["tipo"];
                string idcompra = message.Data["idCompra"];
                string iddistribuidor = message.Data["idDistribuidor"];

                switch (tipo)
                {
                    case "1":
                        Helpers.Settings.Pedidos = true;
                        if (iddistribuidor != null && iddistribuidor!="")
                        {
                            Helpers.Settings.IdDistribuidor = int.Parse(iddistribuidor);
                        }

                         Helpers.Settings.IdCompra = int.Parse(idcompra);

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

                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.O)
                {

                    SendNotification(msg);
                }
                else
                {
                    SendNotificationLower26(msg);
                }

            }

        }

        void SendNotificationLower26(string messageBody)
        {

            Intent intent = new Intent(this, typeof(MainActivity));

            // Create a PendingIntent; we're only using one PendingIntent (ID = 0):
            const int pendingIntentId = 0;
            PendingIntent pendingIntent =
                PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);

            // Instantiate the builder and set notification elements, including pending intent:
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentIntent(pendingIntent)
                .SetContentTitle("EL GAS")
                .SetAutoCancel(true)
                .SetContentText(messageBody)
                .SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 })
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                .SetSmallIcon(Resource.Drawable.ic_launcher)
                .SetChannelId(CHANNEL_ID);

            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);
        }

        void SendNotification(string messageBody)
        {
            int notifyID = 1;
            // The id of the channel. 
            var importance = NotificationImportance.High;

            NotificationChannel mChannel = new NotificationChannel(CHANNEL_ID, "ELGASCLIENTENOTIFICATIONS", importance);

            Intent intent = new Intent(this, typeof(MainActivity));

            // Create a PendingIntent; we're only using one PendingIntent (ID = 0):
            const int pendingIntentId = 0;
            PendingIntent pendingIntent =
                PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);

            // Instantiate the builder and set notification elements, including pending intent:
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentIntent(pendingIntent)
                .SetContentTitle("EL GAS")
                .SetAutoCancel(true)
                .SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 })
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                .SetContentText(messageBody)
                .SetSmallIcon(Resource.Drawable.ic_launcher)
                .SetChannelId(CHANNEL_ID);

            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager mNotificationManager =
             (NotificationManager)GetSystemService(Context.NotificationService);
            mNotificationManager.CreateNotificationChannel(mChannel);

            // Publish the notification:
            mNotificationManager.Notify(notifyID, notification);
        }
    }
}