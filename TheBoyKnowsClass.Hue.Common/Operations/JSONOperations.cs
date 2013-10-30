using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Operations
{
    public static class JSONOperations
    {
        public static object JSONParse(this string myString)
        {
            if (IsArray(myString))
            {
                return JArray.Parse(myString);
            }
            return JObject.Parse(myString);
        }

        public static bool IsArray(string myString)
        {
            if (string.IsNullOrEmpty(myString))
            {
                return false;
            }

            return myString[0] == '[';
        }

        public static string ToJSON<T>(this T hueObject)
            where T : HueObjectBase
        {
            return JsonConvert.SerializeObject(hueObject, Formatting.None, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
        }
    }
}
