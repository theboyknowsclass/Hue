using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    public class BridgeSoftwareUpdate : HueObjectBase
    {
        public BridgeSoftwareUpdate(JObject jObject)
        {
            UpdateState = (int)jObject["updatestate"];
            URL = (string)jObject["url"];
            Text = (string)jObject["text"];
            Notify = (bool)jObject["notify"];
        }

        public int UpdateState { get; private set; }
        public string URL { get; private set; }
        public string Text { get; private set; }
        public bool Notify { get; private set; }
    }
}
