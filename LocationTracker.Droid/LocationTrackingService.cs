using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LocationTracker.Core.Model;
using LocationTracker.Droid.Extensions;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.WindowsAzure.MobileServices;
using Plugin.CurrentActivity;
using Plugin.DeviceInfo;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace LocationTracker.Droid
{
    [Obsolete("This was a testing ground for periodic service starts using AlarmManager. didn't work out as expected")]
    //[Service]
    //[IntentFilter(new [] { StartLocationTrackingServiceIntentActionString })]
    public class LocationTrackingService : IntentService
    {
        public const string StartLocationTrackingServiceIntentActionString =
            "com.sk.ACTION_START_LOCATION_TRACKING_SERVICE";

        private static readonly MobileServiceClient BackendTransactor =
            new MobileServiceClient("https://mobile-41a45763-4209-4cae-bf4b-111148e464c6.azurewebsites.net/");

        private static readonly string DeviceId = CrossDeviceInfo.Current.Id;

        private static IGeolocator Locator => CrossGeolocator.Current;

        public override IBinder OnBind(Intent intent)
        {
            // This is a started service, not a bound service, so we just return null.
            return null;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Analytics.TrackEvent("Destroyed Location Tracking Service");
        }

        protected override async void OnHandleIntent(Intent intent)
        {
            Locator.DesiredAccuracy = 10;

            Analytics.TrackEvent("OnHandleIntent called",
                new Dictionary<string, string>()
                {
                    ["intent.Action"] = intent.Action,
                    ["intent.DataString"] = intent.DataString,
                    ["intent.Extras"] = String(CreateDictionary(intent.Extras))
                });

            try
            {
                var position = await Locator.GetPositionAsync(30000);
                if (position == null)
                {
                    Analytics.TrackEvent("Position returned was null");
                }
                else
                {
                    var locationHistoryTable = BackendTransactor.GetTable<LocationRecord>();
                    await locationHistoryTable.InsertAsync(position.ToLocationRecord(DeviceId));
                    Analytics.TrackEvent("Reported location successfully");
                }
            }
            catch (Exception e)
            {
                Analytics.TrackEvent("Reporting location failed.",
                    new Dictionary<string, string>() { ["e.Type"] = e.GetType().FullName, ["e.Message"] = e.Message });
            }
        }

        public static bool IsScheduledToRunPeriodically(Context context) =>
            PendingIntent.GetBroadcast(context, 0, new Intent(StartLocationTrackingServiceIntentActionString),
                PendingIntentFlags.NoCreate) != null;

        public static void ScheduleToRunPeriodically(Context context)
        {
            var alarmManager = (AlarmManager)context.GetSystemService(AlarmService);

            var locationServiceIntent = new Intent(StartLocationTrackingServiceIntentActionString);
            var pendingIntent = PendingIntent.GetBroadcast(context, 0, locationServiceIntent, PendingIntentFlags.UpdateCurrent);

            alarmManager.SetInexactRepeating(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime() + 60000,
                60000, pendingIntent);
        }

        private static IDictionary<string, string> CreateDictionary(Bundle bundle)
            => bundle?.KeySet().ToDictionary(key => key, key => bundle.Get(key)?.ToString());

        private static string String(IDictionary<string, string> dictionary)
            => dictionary == null ? null : string.Join(";", dictionary.Select(x => x.Key + "=" + x.Value).ToArray());

    }
}