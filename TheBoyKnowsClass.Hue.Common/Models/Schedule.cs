using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;
using TheBoyKnowsClass.Hue.Common.Models.Factories;
using TheBoyKnowsClass.Hue.Common.Operations;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    public class Schedule : HueConnectedObjectBase
    {
        public Schedule()
        {
            
        }

        public Schedule(Command command, DateTime? time)
        {
            Command = command;
            Time = time;
        }

        public Schedule(JObject jObject, HueConnection connection, string id)
            : base(connection, id)
        {
            Name = (string)jObject["name"];
            Description = (string)jObject["description"];

            if (jObject["command"] != null)
            {
                var command = HueObjectFactory.CreateHueObject(jObject["command"].ToString(), HueObjectType.Command) as Command;
                if (command != null)
                {
                    Command = command;
                }
            }

            Time = (DateTime?)jObject["time"];

        }

        new public string ID { get { return base.ID; }}
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("command")]
        public Command Command { get; set; }
        [JsonIgnore]
        public DateTime? Time { get; set; }
        [JsonProperty("time")]
        public string TimeString
        {
            get { return Time.HasValue ? Time.Value.ToString("yyyy-MM-ddTHH:mm:ss") : null; }
        }


        #region Schedule API

        public async Task<HueObjectBase> GetAttributesAsync()
        {
            HueActionDelegate method = async delegate
            {
                string returnString = await Context.GetAsync(Context.ScheduleURI(ID));
                return HueObjectFactory.CreateHueObject(JObject.Parse(returnString), ID, Context, HueObjectType.Schedule);
            };
            return await Context.LoginWrapperAsync(method);
        }

        public async Task<HueObjectBase> SetAttributesAsync(Schedule schedule)
        {
            HueActionDelegate method = async delegate
            {
                string newScheduleString = schedule.ToJSON();
                string returnString = await Context.PutAsync(Context.ScheduleURI(ID), newScheduleString);
                return HueObjectFactory.CreateHueObject(returnString, HueObjectType.Success);
            };
            return await Context.LoginWrapperAsync(method);
        }

        #endregion
    }
}
