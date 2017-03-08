using Android.App;
using Android.OS;
using LocationTracker.Core;
using Microsoft.Azure.Mobile;
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
        }
    }
}

