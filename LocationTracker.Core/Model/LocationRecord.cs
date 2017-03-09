using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LocationTracker.Core.Model
{
    public class LocationRecord
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "accuracy")]
        public double Accuracy { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public double Longitude { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    public static class LocationExtensions
    {
        public static JObject ToJObject(this LocationRecord locationRecord) => JObject.FromObject(locationRecord);
    }
}
