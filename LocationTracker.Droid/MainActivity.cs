using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using LocationTracker.Core;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Plugin.Permissions;
using Xamarin.Forms.Platform.Android;

namespace LocationTracker.Droid
{
    [Activity(Label = "Location Tracker", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            Xamarin.Forms.Forms.Init(this, bundle);
            MobileCenter.Configure("b19a97d0-8336-48cb-8e95-6269359dcf44");

            LoadApplication(new App());

            if (!IsLocationTrackingServiceRunning)
                StartService(new Intent(this, typeof(LocationTrackingService)));

            //TODO: auto start service on device bootup

            Analytics.TrackEvent("Created MainActivity");
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private bool IsLocationTrackingServiceRunning
            => ((ActivityManager) GetSystemService(ActivityService)).GetRunningServices(int.MaxValue)
                .Any(service => service.Service.ClassName == (typeof(LocationTrackingService)).Name);
    }
}

