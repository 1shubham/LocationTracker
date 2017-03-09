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
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;

namespace LocationTracker.Droid
{
    public static class MobileServicesManager
    {
        public static void SafeInit()
        {
            if (!MobileCenter.Configured)
                MobileCenter.Configure("b19a97d0-8336-48cb-8e95-6269359dcf44");

            MobileCenter.Enabled = true;
            Analytics.Enabled = true;
        }
    }
}