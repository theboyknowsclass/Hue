using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Models.Factories;

namespace TheBoyKnowsClass.Hue.Common.Models.BaseClasses
{
    public class HueObjectCollectionBase<T> : HueObjectBase, ICollection<T>
        where T : HueObjectBase
    {
        public Dictionary<string, T> Dictionary { get; private set; }

        public HueObjectCollectionBase(JObject jObject) : this(jObject, null)
        {
        }

        public HueObjectCollectionBase(JObject jObject, HueConnection connection)
        {
            Dictionary = new Dictionary<string, T>();

            foreach (KeyValuePair<string, object> kvp in JsonConvert.DeserializeObject<Dictionary<string, object>>(jObject.ToString()))
            {
                try
                {
                    JObject myObject = JObject.FromObject(kvp.Value);

                    HueObjectBase item = HueObjectFactory.CreateHueObject(myObject, kvp.Key, connection, HueObjectFactory.GetType(typeof(T)));

                    if (item is T)
                    {
                        Dictionary.Add(kvp.Key, (T)item);
                    }
                }
                catch (ArgumentException exception)
                {
                    Debug.WriteLine("Error when converting {0} : {1} - {2}", kvp.Key, kvp.Value, exception.Message);
                }
            }
        }

        public HueObjectCollectionBase(JArray jArray, HueConnection connection)
        {
            Dictionary = new Dictionary<string, T>();

            foreach (object myObject in JsonConvert.DeserializeObject<List<object>>(jArray.ToString()))
            {
                try
                {
                    JObject myJObject = JObject.FromObject(myObject);

                    HueObjectBase item = HueObjectFactory.CreateHueObject(myJObject, connection, HueObjectFactory.GetType(typeof(T)));

                    if (item is T)
                    {
                        Dictionary.Add(Dictionary.Count.ToString(CultureInfo.InvariantCulture), (T)item);
                    }
                }
                catch (ArgumentException exception)
                {
                    Debug.WriteLine("Error when converting {0} - {1}", myObject, exception.Message);
                }
            }
        }  

        public HueObjectCollectionBase()
        {
            Dictionary = new Dictionary<string, T>();
        }

        public void Add(T item)
        {
            AutoAdd(item);
        }

        public void Clear()
        {
            Dictionary.Clear();
        }

        public bool Contains(T item)
        {
            return Dictionary.Values.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Dictionary.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return RemoveByValue(item);
        }

        public int Count
        {
            get { return Dictionary.Count; }
        }

        public bool IsReadOnly { get; private set; }

        public T GetItem(string key)
        {
            return Dictionary[key];
        }

        public T FirstOrDefault()
        {
            return (from v in Dictionary.Values select v).FirstOrDefault();
        }

        public HueObjectCollectionBase<T> AutoAdd(T item)
        {
            Dictionary.Add(Dictionary.Count.ToString(CultureInfo.InvariantCulture), item);
            return this;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Dictionary.Values.GetEnumerator();
        }

        public override string ToString()
        {
            return Dictionary.Values.Aggregate(" | ", (current, item) => current + item.ToString());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Dictionary.Values.GetEnumerator();
        }

        private bool RemoveByValue(T value)
        {
            var itemsToRemove = new List<string>();

            foreach (var pair in Dictionary)
            {
                if (pair.Value.Equals(value))
                {
                    itemsToRemove.Add(pair.Key);
                }
            }

            foreach (string item in itemsToRemove)
            {
                Dictionary.Remove(item);
            }

            return itemsToRemove.Count != 0;
        }
    }
}
