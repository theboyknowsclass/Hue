using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    public class Client : HueObjectBase
    {
        public Client(JObject jObject, string id) : base(id)
        {
            ClientType = (string)jObject["name"];
            LastUsedDate = (DateTime)jObject["last use date"];
            CreatedDate = (DateTime)jObject["create date"];
        }

        public Client(string clientType, string id) : base(id)
        {
            ClientType = clientType;
        }

        [JsonProperty("devicetype")]
        public string ClientType { get; private set; }
        [JsonProperty("username")]
        new public string ID
        { get { return base.ID;
        } }
        [JsonProperty("last use date")]
        public DateTime? LastUsedDate { get; private set; }
        [JsonProperty("create date")]
        public DateTime? CreatedDate { get; private set; }
    }
}
