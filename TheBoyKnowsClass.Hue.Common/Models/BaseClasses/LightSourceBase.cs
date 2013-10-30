using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Interfaces;
using TheBoyKnowsClass.Hue.Common.Models.Factories;

namespace TheBoyKnowsClass.Hue.Common.Models.BaseClasses
{
    public abstract class LightSourceBase : HueConnectedObjectBase, ILightSource
    {
        protected LightSourceBase()
        {}

        protected LightSourceBase(string id) : base(id)
        {}

        protected LightSourceBase(HueConnection context, string id) : base(context, id)
        {}


        new public string ID { get { return base.ID; } }
        public State State { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        protected abstract string URI { get; }
        protected abstract string StateURI { get; }

        public async Task<HueObjectBase> SetNameAsync<T>(string newName)
            where T : ILightSource, new()
        {
            var rv = await SetAttributesAsync<T>(ID, new T { Name = newName });
            if (!rv.IsError())
            {
                Name = newName;
            }
            return rv;
        }


        protected async Task<HueObjectBase> GetAttributesAsync<T>(string id)
        {
            HueActionDelegate method = async delegate
            {
                string returnString = await Context.GetAsync(URI);
                return HueObjectFactory.CreateHueObject(JObject.Parse(returnString), id, Context, HueObjectFactory.GetType(typeof(T)));
            };
            return await Context.LoginWrapperAsync(method);
        }

        protected async Task<HueObjectBase> SetAttributesAsync<T>(string id, T myObject)
        {
            HueActionDelegate method = async delegate
            {
                string newObjectString = JsonConvert.SerializeObject(myObject, Formatting.None,
                                                                    new JsonSerializerSettings
                                                                    {
                                                                        NullValueHandling = NullValueHandling.Ignore
                                                                    });
                string returnString = await Context.PutAsync(URI, newObjectString);
                return HueObjectFactory.CreateHueObject(returnString, HueObjectType.Success);
            }
                ;
            return await Context.LoginWrapperAsync(method);
        }

        protected async Task<HueObjectBase> SetStateAsync(string id, State state)
        {
            string uri = StateURI;

            HueActionDelegate method = async delegate
            {
                string newStateString = JsonConvert.SerializeObject(state, Formatting.None,
                                                                    new JsonSerializerSettings
                                                                    {
                                                                        NullValueHandling =
                                                                            NullValueHandling.Ignore
                                                                    });
                string returnString = await Context.PutAsync(uri, newStateString);
                return HueObjectFactory.CreateHueObject(returnString, HueObjectType.Success);
            };
            return await Context.ThrottleWrapperAsync(new HueConnection.QueueItem { Time = DateTime.Now, Path = uri, Action = method });
        }

    }
}
