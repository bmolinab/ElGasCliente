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

namespace ElGas.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
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

                switch (tipo)
                {
                    case "1":
                        //Helpers.Settings.VenderGas = true;
                       // Helpers.Settings.IdCompra = int.Parse(idcompra);

                        break;
                }

                SendNotification(msg);

            }

        }

        void SendNotification(string messageBody)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
            long[] v = { 500, 1000 };

            var notificationBuilder = new Notification.Builder(this)
                        .SetContentTitle("El Gas")
                        .SetSmallIcon(Resource.Drawable.ic_launcher)
                        .SetContentText(messageBody)
                        .SetAutoCancel(true)
                        .SetContentIntent(pendingIntent)
                        .SetVibrate(v)
                        .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))

            ;

            var notificationManager = NotificationManager.FromContext(this);


            notificationManager.Notify(1, notificationBuilder.Build());
        }
    }
}