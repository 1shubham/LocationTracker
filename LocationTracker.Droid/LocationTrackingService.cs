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
using Plugin.CurrentActivity;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace LocationTracker.Droid
{
    [Service]
    public class LocationTrackingService : Service
    {
        private static IGeolocator Locator => CrossGeolocator.Current;

        public override IBinder OnBind(Intent intent)
        {
            // This is a started service, not a bound service, so we just return null.
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //Initialize locator data, if any
            Locator.DesiredAccuracy = 10;

            Analytics.TrackEvent("Created Location Tracking Service");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            //Stop listening to location updates
            if (Locator.IsListening)
            {
                Locator.PositionChanged -= ProcessLocationUpdate;
                Locator.PositionError -= ProcessLocationError;

                var stoppedListeningTask = Locator.StopListeningAsync();

                stoppedListeningTask.Wait();
                if (stoppedListeningTask.IsCompleted)
                {
                    if (stoppedListeningTask.Result)
                    {
                        Locator.PositionChanged -= ProcessLocationUpdate;
                        Locator.PositionError -= ProcessLocationError;
                    }
                    else
                    {
                        Analytics.TrackEvent("Failed to stop listening to location updates.");
                    }
                }
                else
                {
                    Analytics.TrackEvent(
                        $"stoppedListeningTask did not complete and ended up with status: {stoppedListeningTask.Status} instead with possible exception message: {stoppedListeningTask.Exception?.Message}");
                }
            }
            else
            {
                Analytics.TrackEvent("Attempted to stop listening to location updates while currently not listening.");
            }

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

            //Start listening to location updates
            if (Locator.IsGeolocationAvailable)
            {
                if (Locator.IsGeolocationEnabled)
                {
                    if (!Locator.IsListening)
                    {
                        var startedListeningTask = Locator.StartListeningAsync(5000, 5, true);

                        startedListeningTask.Wait();
                        if (startedListeningTask.IsCompleted)
                        {
                            if (startedListeningTask.Result)
                            {
                                Locator.PositionChanged += ProcessLocationUpdate;
                                Locator.PositionError += ProcessLocationError;
                            }
                            else
                            {
                                Analytics.TrackEvent("Failed to start listening to location updates.");   
                            }
                        }
                        else
                        {
                            Analytics.TrackEvent(
                                $"startedListeningTask did not complete and ended up with status: {startedListeningTask.Status} instead with possible exception message: {startedListeningTask.Exception?.Message}");
                        }
                    }
                    else
                    {
                        Analytics.TrackEvent("Attempted to start listening to location updates while already listening.");
                    }
                }
                else
                {
                    Analytics.TrackEvent("Geolocation not enabled.");
                }
            }
            else
            {
                Analytics.TrackEvent("Geolocation not available.");
            }

            return StartCommandResult.RedeliverIntent;
        }

        private void ProcessLocationUpdate(object sender, PositionEventArgs args)
        {
            //TODO: process location updates
            Application.SynchronizationContext.Post(_ =>
            {
                Toast.MakeText(ApplicationContext, "Received a location update.", ToastLength.Short).Show();
            }, null);
        }

        private static void ProcessLocationError(object sender, PositionErrorEventArgs args)
        {
            Analytics.TrackEvent("Location update error",
                new Dictionary<string, string>() {["Error"] = args.Error.ToString()});
        }

        private static IDictionary<string, string> CreateDictionary(Bundle bundle)
            => bundle?.KeySet().ToDictionary(key => key, key => bundle.Get(key)?.ToString());

        private static string String(IDictionary<string, string> dictionary)
            => dictionary == null ? null : string.Join(";", dictionary.Select(x => x.Key + "=" + x.Value).ToArray());
    }
}