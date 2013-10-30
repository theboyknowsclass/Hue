using System;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;
using TheBoyKnowsClass.Hue.Common.Operations;
using TheBoyKnowsClass.Hue.Common.Properties;

namespace TheBoyKnowsClass.Hue.Common.Models.Factories
{
    public static class HueObjectFactory
    {
        public static HueObjectBase CreateHueObject(string myString, HueObjectType type)
        {
            return CreateHueObject(myString, null, type);
        }

        public static HueObjectBase CreateHueObject(string myString, HueConnection hueConnection, HueObjectType type)
        {
            var myObject = myString.JSONParse();

            if (myObject is JObject)
            {
                return CreateHueObject(myObject as JObject, hueConnection, type);
            }

            if (myObject is JArray)
            {
                return CreateHueObject(myObject as JArray, hueConnection, type);
            }

            throw new ArgumentException(Resources.NoObjectConstructorForType, myObject.GetType().ToString());
        }

        private static HueObjectBase CreateHueObject(JArray jArray, HueConnection hueConnection, HueObjectType type)
        {
            if (jArray[0]["error"] != null)
            {
                type = HueObjectType.Error;
            }

            switch (type)
            {
                case HueObjectType.Success:
                    return new HueObjectCollectionBase<Success>(jArray, hueConnection);
                case HueObjectType.Error:
                    return new HueObjectCollectionBase<Error>(jArray, hueConnection);
                case HueObjectType.Bridge:
                    return new HueObjectCollectionBase<Bridge>(jArray, hueConnection);
                default:
                    throw new ArgumentOutOfRangeException(String.Format(Resources.NoCollectionConstructorForType, type));
            }
        }

        public static HueObjectBase CreateHueObject(JObject jObject, HueConnection hueConnection, HueObjectType type)
        {
            if (jObject["error"] != null)
            {
                return new Error(jObject);
            }

            switch (type)
            {
                case HueObjectType.Success:
                    return new Success(jObject);
                case HueObjectType.Bridge:
                    return new Bridge(jObject, hueConnection);
                case HueObjectType.BridgeConfig:
                    return new BridgeConfig(jObject);
                case HueObjectType.BridgeSoftwareUpdate:
                    return new BridgeSoftwareUpdate(jObject);
                case HueObjectType.Light:
                    return new HueObjectCollectionBase<Light>(jObject, hueConnection);
                case HueObjectType.LastAddedLights:
                    return new LastAddedLights(jObject);
                case HueObjectType.LightState:
                    return new State(jObject);
                case HueObjectType.Group:
                    return new HueObjectCollectionBase<Group>(jObject, hueConnection);
                case HueObjectType.Schedule:
                    return new HueObjectCollectionBase<Schedule>(jObject, hueConnection);
                case HueObjectType.Command:
                    return new Command(jObject);
                default:
                    throw new ArgumentOutOfRangeException(String.Format(Resources.NoObjectConstructorForType, type));
            }
        }

        public static HueObjectBase CreateHueObject(JObject jObject, string id, HueConnection hueConnection, HueObjectType type)
        {
            if (jObject["error"] != null)
            {
                return new Error(jObject);
            }

            switch (type)
            {
                case HueObjectType.Light:
                    return new Light(jObject, hueConnection, id);
                case HueObjectType.Client:
                    return new Client(jObject, id);
                case HueObjectType.Group:
                    return new Group(jObject, hueConnection, id);
                case HueObjectType.Schedule:
                    return new Schedule(jObject, hueConnection, id);
                default:
                    return new Error(jObject);
            }
        }

        public static HueObjectType GetType(Type type)
        {
            if (type == typeof(Error))
            {
                return HueObjectType.Error;
            }

            if (type == typeof(Success))
            {
                return HueObjectType.Success;
            }

            if (type == typeof(Bridge))
            {
                return HueObjectType.Bridge;
            }

            if (type == typeof(Client))
            {
                return HueObjectType.Client;
            }

            if (type == typeof(Light))
            {
                return HueObjectType.Light;
            }

            if (type == typeof(Group))
            {
                return HueObjectType.Group;
            }

            if (type == typeof(Schedule))
            {
                return HueObjectType.Schedule;
            }

            throw new ArgumentOutOfRangeException(String.Format(Resources.NoObjectConstructorForType, type));
        }

        public static bool IsError(this HueObjectBase hueObject)
        {
            return hueObject is Error || hueObject is HueObjectCollectionBase<Error>;
        }

        public static HueObjectCollectionBase<T> To<T>(this HueObjectBase hueObject) 
            where T : HueObjectBase
        {
            if (hueObject is T)
            {
                return new HueObjectCollectionBase<T>{(T) hueObject};
            }
            
            else if (hueObject is HueObjectCollectionBase<T>)
            {
                return (HueObjectCollectionBase<T>)hueObject;
            }

            return null;
        }
    }
}
