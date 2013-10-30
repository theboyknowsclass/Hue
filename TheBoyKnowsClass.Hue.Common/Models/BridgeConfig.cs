using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;
using TheBoyKnowsClass.Hue.Common.Models.Factories;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    [DataContract]
    public class BridgeConfig : HueObjectBase
    {
        public BridgeConfig()
        {}

        public BridgeConfig(JObject jObject)
        {
            Name = (string)jObject["name"];
            MAC = (string)jObject["mac"];
            DHCP = (bool?)jObject["dhcp"];
            IPAddress = (string)jObject["ipaddress"];
            NetMask = (string)jObject["netmask"];
            Gateway = (string)jObject["gateway"];
            ProxyAddress = (string)jObject["proxyaddress"];
            ProxyPort = (int?)jObject["proxyport"];
            LastUpdated = (DateTime?)jObject["UTC"];
            WhiteList = jObject["whitelist"] != null ? new HueObjectCollectionBase<Client>(JObject.FromObject(jObject["whitelist"])) : new HueObjectCollectionBase<Client>();
            SoftwareVersion = (string)jObject["swversion"];
            if (jObject["swupdate"] != null)
            {
                HueObjectBase softwareUpdate = HueObjectFactory.CreateHueObject(jObject["swupdate"].ToString(), HueObjectType.BridgeSoftwareUpdate);
                var update = softwareUpdate as BridgeSoftwareUpdate;
                if (update != null)
                {
                    SoftwareUpdate = update;
                }
            }
            LinkButton = (bool?)jObject["linkbutton"];
            PortalServices = (bool?)jObject["portalservices"];
        }

        [JsonProperty("name")]
        public string Name { get; set; }
        public string MAC { get; private set; }
        [JsonProperty("dhcp")]
        public bool? DHCP { get; set; }
        [JsonProperty("ipaddress")]
        public string IPAddress { get; set; }
        [JsonProperty("netmask")]
        public string NetMask { get; set; }
        [JsonProperty("gateway")]
        public string Gateway { get; set; }
        [JsonProperty("proxyaddress")]
        public string ProxyAddress { get; set; }
        [JsonProperty("proxyport")]
        public int? ProxyPort { get; set; }
        public DateTime? LastUpdated { get; private set; }
        public HueObjectCollectionBase<Client> WhiteList { get; private set; }
        public string SoftwareVersion { get; private set; }
        public BridgeSoftwareUpdate SoftwareUpdate { get; private set; }
        [JsonProperty("linkbutton")]
        public bool? LinkButton { get; set; }
        [JsonProperty("portalservices")]
        public bool? PortalServices { get; set; }


    }
}
