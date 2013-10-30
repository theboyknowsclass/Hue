using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    public class State : HueObjectBase
    {
        public State()
        {
        }

        public State(JObject jObject)
        {
            On = (bool?)jObject["on"];
            Brightness = (int?)jObject["bri"];
            Hue = (int?)jObject["hue"];
            Saturation = (int?)jObject["sat"];
            
            if (jObject["xy"] != null)
            {
                CIEColor = jObject["xy"].ToObject<List<double>>();
            }

            ColorTemperature = (int?)jObject["ct"];
            Alert = (string)jObject["alert"];
            Effect = (string)jObject["effect"];
            ColorMode = (string)jObject["colormode"];
            Reachable = (bool?)jObject["reachable"];
            TransitionTime = (int?)jObject["transitiontime"];
        }

        [JsonProperty("on")]
        public bool? On { get; set; }
        [JsonProperty("bri")]
        public int? Brightness { get; set; }
        [JsonProperty("hue")]
        public int? Hue { get; set; }
        [JsonProperty("sat")]
        public int? Saturation { get; set; }
        [XmlElement(IsNullable = false)]
        [JsonProperty("xy")]
        public List<double> CIEColor { get; set; }
        [JsonProperty("ct")]
        public int? ColorTemperature { get; set; }
        [JsonProperty("alert")]
        public string Alert { get; set; }
        [JsonProperty("effect")]
        public string Effect { get; set; }
        [JsonIgnore]
        public string ColorMode { get; set; }
        [XmlIgnore] 
        public bool? Reachable { get; private set; }
        [JsonProperty("transitiontime")]
        public int? TransitionTime { get; set; }

        public bool ShouldSerializeCIEColor()
        {
            if (CIEColor != null && CIEColor.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
