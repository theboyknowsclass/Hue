using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    public class Command : HueObjectBase
    {
        public Command(JObject jObject)
        {
            Address = (string)jObject["address"];
            Method = (string)jObject["method"];
            if (jObject["body"] != null)
            {
                Body = jObject["body"].ToObject<Dictionary<string, object>>();
            }
        }

        public Command()
        {
            Body = new Dictionary<string, object>();
        }

        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("body")]
        public Dictionary<string, object> Body { get; set; }
    }
}
