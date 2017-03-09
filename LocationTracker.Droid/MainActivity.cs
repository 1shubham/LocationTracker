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
            MobileServicesManager.SafeInit();

            LoadApplication(new App());
            
            if (!PersistentLocationTrackingService.IsRunning(ApplicationContext))
                StartService(new Intent(ApplicationContext, typeof(PersistentLocationTrackingService)));

            Analytics.TrackEvent("Created MainActivity");
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

