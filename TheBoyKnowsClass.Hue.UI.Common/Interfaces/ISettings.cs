using TheBoyKnowsClass.Common.Models;
using TheBoyKnowsClass.Hue.UI.Common.Models;

namespace TheBoyKnowsClass.Hue.UI.Common.Interfaces
{
    public interface ISettings
    {
        Setting<string> Bridge { get; set; }
        Setting<string> DeviceType { get; set; }
        Setting<string> ApplicationID { get; set; }
        SerializableDictionary<string, SerializableDictionary<int, Scene>> Scenes { get; set; }
        void Save();
    }
}
