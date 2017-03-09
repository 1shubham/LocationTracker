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
using LocationTracker.Core.Model;
using Plugin.Geolocator.Abstractions;

namespace LocationTracker.Droid.Extensions
{
    public static class PositionExtensions
    {
        public static LocationRecord ToLocationRecord(this Position pos, string deviceId) => new LocationRecord
        {
            Accuracy = pos.Accuracy,
            DeviceId = deviceId,
            Latitude = pos.Latitude,
            Longitude = pos.Longitude,
            Timestamp = pos.Timestamp
        };
    }
}