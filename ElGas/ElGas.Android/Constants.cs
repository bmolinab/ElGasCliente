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

namespace ElGas.Droid
{
    public class Constants
    {
        public const string SenderID = "5570742533"; // Google API Project Number
        public const string ListenConnectionString = "Endpoint=sb://notificacionesdesarrollo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=B10SdmICrxrk4RAZ3abyGBB3SdrntdmW+iKImvByLIQ=";
        public const string NotificationHubName = "notificacionesds";
    }
}