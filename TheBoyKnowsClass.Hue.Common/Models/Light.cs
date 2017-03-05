using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;
using TheBoyKnowsClass.Hue.Common.Models.Factories;
using System.Collections.Generic;
using Newtonsoft.Json;
using TheBoyKnowsClass.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Models.Attributes;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    public class Light : LightSourceBase
    {
        public Light()
        {
            SupportedColorModes = new List<ColorMode>();
        }

        public Light(string id) : base(id)
        {
            SupportedColorModes = new List<ColorMode>();
        }

        public Light(HueConnection context, string id) : base(context, id)
        {
            
        }

        public Light(JObject jObject, HueConnection context, string id) : this(context, id)
        {
            if (jObject["state"] != null)
            {
                var lightState = HueObjectFactory.CreateHueObject(jObject["state"].ToString(), HueObjectType.LightState) as State;
                if (lightState != null)
                {
                    State = lightState;
                }
            }

            Type = (string)jObject["type"];

            LightType = EnumHelper.GetEnumWithDescription<LightType>(Type);
            SupportsColorModeAttribute supportsColorModeAttribute = LightType.GetEnumAttribute<SupportsColorModeAttribute>();

            if (supportsColorModeAttribute != null)
            {
                SupportedColorModes = supportsColorModeAttribute.Modes;
            }
            else
            {
                SupportedColorModes = new List<ColorMode>();
            }

            Name = (string)jObject["name"];
            ModelID = (string)jObject["modelid"];
            SoftwareVersion = (string)jObject["swversion"];
        }

        public string Type { get; private set; }

        public LightType? LightType { get; private set; }

        [JsonIgnore]
        public bool CanDim { get; private set; }

        [JsonIgnore]
        public IEnumerable<ColorMode> SupportedColorModes { get; private set;}

        protected override string URI
        {
            get { return Context.LightURI(ID); }
        }

        protected override string StateURI
        {
            get { return Context.LightStateURI(ID); }
        }

        public string ModelID { get; private set; }
        public string SoftwareVersion { get; private set; }
        public PointSymbol PointSymbol { get; set; }

       

        public async Task<HueObjectBase> SetAlertAsync()
        {
            return await SetStateAsync(ID, new State { Alert = "lselect" });
        }

        public async Task<HueObjectBase> CancelAlertAsync()
        {
            return await SetStateAsync(ID, new State { Alert = "none" });
        }

        public async Task<HueObjectBase> GetAttributesAsync()
        {
            return await GetAttributesAsync<Light>(ID);
        }

        public async Task<bool> RefreshAttributesAsync()
        {
            var rv = await GetAttributesAsync<Light>(ID);

            if (rv.IsError())
            {
                return false;
            }

            State = ((Light) rv).State;

            return true;
        }


        public async Task<HueObjectBase> SetStateAsync(State state)
        {
            return await SetStateAsync(ID, state);
        }
    }
}
