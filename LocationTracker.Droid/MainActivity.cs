using Android.App;
using Android.OS;
using LocationTracker.Core;
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
            
            LoadApplication(new App());
        }
    }
}

