using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;
using TheBoyKnowsClass.Hue.Common.Models.Factories;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    public class Group : LightSourceBase
    {
        public Group()
        {
            LightsIDs = new List<string>();
        }

        public Group(JObject jObject, HueConnection context, string id) : base(context, id)
        {
            Name = (string)jObject["name"];

            if (jObject["action"] != null)
            {
                var lightState = HueObjectFactory.CreateHueObject(jObject["action"].ToString(), HueObjectType.LightState) as State;
                if (lightState != null)
                {
                    State = lightState;
                }
            }

            Lights = new HueObjectCollectionBase<Light>();

            if (jObject["lights"] != null)
            {
                LightsIDs = jObject["lights"].ToObject<List<string>>();
            }

        }

        [JsonProperty("lights")]
        public List<string> LightsIDs
        {
            get
            {
                if (Lights != null)
                {
                    return (from l in Lights.Dictionary select l.Value.ID).ToList();
                }
                return null;
            }
            set
            {
                if (Lights == null)
                {
                    Lights = new HueObjectCollectionBase<Light>();
                }

                foreach (string lightID in value)
                {
                    if (!Lights.Dictionary.ContainsKey(lightID))
                    {
                        Light light;

                        if (Context != null)
                        {
                            light = new Light(Context, lightID);
                        }
                        else
                        {
                            light = new Light(lightID);
                        }


                        Lights.Dictionary.Add(lightID, light);
                    }
                }
            }
        }

        [JsonIgnore]
        public HueObjectCollectionBase<Light> Lights { get; private set; }

        protected override string URI
        {
            get { return Context.GroupURI(ID); }
        }
        protected override string StateURI
        {
            get { return Context.GroupStateURI(ID); }
        }

        public async Task<HueObjectBase> GetAttributesAsync()
        {
            return await GetAttributesAsync<Group>(ID);
        }

        public async Task<HueObjectBase> SetStateAsync(State state)
        {
            return await SetStateAsync(ID, state);
        }

        public async Task<HueObjectBase> SetAttributesAsync()
        {
            return await SetAttributesAsync(ID, this);
        }

        public async Task<HueObjectBase> AddLightByID(string lightID)
        {
            if (Lights.Dictionary.ContainsKey(lightID))
            {
                return new Error(-1, "", "Light is already part of this group");
            }

            var ids = LightsIDs;
            ids.Add(lightID);

            var rv = await SetAttributesAsync<Group>(ID, new Group { LightsIDs = ids });
            
            if (!rv.IsError())
            {
                await GetAttributesAsync();
            }

            return rv;
        }
    }
}
