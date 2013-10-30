using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    public class Success : HueObjectBase
    {
        public Success(JObject jObject)
        {
            if (jObject["success"].Type == JTokenType.Object)
            {
                KeyValuePair<string, object> success = (from kvps in JsonConvert.DeserializeObject<Dictionary<string, object>>(jObject["success"].ToString())
                                                        select kvps).FirstOrDefault();
                Path = success.Key;
                PathValue = success.Value.ToString();
            }
            else
            {
                Path = (string)jObject["success"];
            }
        }

        public Success(string path, string pathvalue)
        {
            Path = path;
            PathValue = pathvalue;
        }

        public string Path { get; private set; }
        public string PathValue { get; private set; }

        public override string ToString()
        {
            return string.Format("success : {0} {1}", Path, PathValue);
        }
    }
}
