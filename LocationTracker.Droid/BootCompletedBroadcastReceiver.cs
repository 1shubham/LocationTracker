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
using Microsoft.Azure.Mobile.Analytics;

namespace LocationTracker.Droid
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootCompletedBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            MobileServicesManager.SafeInit();

            if (intent.Action == Intent.ActionBootCompleted)
            {
                Analytics.TrackEvent("Received ActionBootCompleted Intent");

                if (!PersistentLocationTrackingService.IsRunning(context.ApplicationContext))
                    context.StartService(new Intent(context.ApplicationContext, typeof(PersistentLocationTrackingService)));
            }
        }
    }
}