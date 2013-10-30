using TheBoyKnowsClass.Common.Models;
using TheBoyKnowsClass.Hue.UI.Common.Interfaces;

namespace TheBoyKnowsClass.Hue.UI.Common.Models
{
    public class CloudSettings : ISettings 
    {
        public static CloudSettings Default
        {
            get { return InstanceCreator<CloudSettings>.Instance; }
        }

        public Setting<string> Bridge { get; set; }
        public Setting<string> DeviceType { get; set; }
        public Setting<string> ApplicationID { get; set; }
        public SerializableDictionary<string, SerializableDictionary<int, Scene>> Scenes { get; set; }
        
        public void Save()
        {
            throw new System.NotImplementedException();
        }
    }
}
