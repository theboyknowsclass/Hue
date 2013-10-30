using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    public class LastAddedLights : HueObjectCollectionBase<Light>
    {
        public LastAddedLights(JObject jObject) : base(jObject, null)
        {
            LastScan = (string)jObject["lastscan"];
        }

        public LastAddedLights(JArray jArray)
            : base(jArray, null)
        {
            LastScan = (string)jArray["lastscan"];
        }

        public string LastScan { get; private set; }

        public bool Scanning { get { return LastScan == "active"; }}
    }
}
