using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LocationTracker.Core.Model;
using LocationTracker.Droid.Extensions;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.WindowsAzure.MobileServices;
using Plugin.DeviceInfo;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace LocationTracker.Droid
{
    [Service]
    public class PersistentLocationTrackingService : Service
    {
        private static readonly MobileServiceClient BackendTransactor =
            new MobileServiceClient("https://mobile-41a45763-4209-4cae-bf4b-111148e464c6.azurewebsites.net/");
        private static readonly string DeviceId = CrossDeviceInfo.Current.Id;

        private Timer _timer;

        public override IBinder OnBind(Intent intent)
        {
            // This is a started service, not a bound service, so we just return null.
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            MobileServicesManager.SafeInit();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            _timer.Dispose();
            _timer = null;

            Analytics.TrackEvent("Destroyed Location Tracking Service");
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Analytics.TrackEvent("OnStartCommand called",
                new Dictionary<string, string>()
                {
                    ["StartCommandFlags"] = flags.ToString(),
                    ["startId"] = startId.ToString(),
                    ["intent.Action"] = intent.Action,
                    ["intent.DataString"] = intent.DataString,
                    ["intent.Extras"] = String(CreateDictionary(intent.Extras))
                });

            _timer = new Timer(TrackAndReportLocation, null, 0, 60000);

            return StartCommandResult.RedeliverIntent;
        }

        private async void TrackAndReportLocation(object state)
        {
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

        public static bool IsRunning(Context context)
            => ((ActivityManager) context.GetSystemService(ActivityService)).GetRunningServices(int.MaxValue)
                .Any(service => service.Service.ClassName == (typeof(PersistentLocationTrackingService)).Name);

        private static IDictionary<string, string> CreateDictionary(Bundle bundle)
            => bundle?.KeySet().ToDictionary(key => key, key => bundle.Get(key)?.ToString());

        private static string String(IDictionary<string, string> dictionary)
            => dictionary == null ? null : string.Join(";", dictionary.Select(x => x.Key + "=" + x.Value).ToArray());

        private static IGeolocator Locator => CrossGeolocator.Current;
    }
}