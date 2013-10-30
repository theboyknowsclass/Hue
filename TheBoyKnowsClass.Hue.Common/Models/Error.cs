using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    [DataContract]
    public class Error : HueObjectBase
    {
        public Error(JObject jObject)
        {
            Type = int.Parse((string)jObject["error"]["type"]);
            Address = (string)jObject["error"]["address"];
            Description = (string)jObject["error"]["description"];
        }

        public Error(int type, string address, string message)
        {
            Type = type;
            Address = address;
            Description = message;
        }

        [DataMember(Name = "type")]
        public int Type { get; private set; }
        [DataMember(Name = "address")]
        public string Address { get; private set; }
        [DataMember(Name = "description")]
        public string Description { get; private set; }
    }
}
