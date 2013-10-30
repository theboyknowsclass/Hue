using System.Configuration;
using TheBoyKnowsClass.Common.Models;
using TheBoyKnowsClass.Hue.UI.Common.Interfaces;
using TheBoyKnowsClass.Hue.UI.Common.Models;

namespace TheBoyKnowsClass.Hue.UI.Common.Desktop.Models
{
    public class LocalSettings : ApplicationSettingsBase, ISettings
    {
        public static LocalSettings Default
        {
            get { return InstanceCreator<LocalSettings>.Instance; }
        }

        [UserScopedSetting]
        public string SkyDriveLogin
        {
            get { return (string)this["SkyDriveLogin"]; }
            set { this["SkyDriveLogin"] = value; }
        }

        [UserScopedSetting]
        public Setting<string> Bridge
        {
            get { return (Setting<string>)this["Bridge"]; }
            set { this["Bridge"] = value; }
        }

        [UserScopedSetting]
        public Setting<string> DeviceType
        {
            get { return (Setting<string>)this["DeviceType"]; }
            set { this["DeviceType"] = value; }
        }

        [UserScopedSetting]
        public Setting<string> ApplicationID
        {
            get { return (Setting<string>)this["ApplicationID"]; }
            set { this["ApplicationID"] = value; }
        }

        [UserScopedSetting]
        public SerializableDictionary<string,SerializableDictionary<int, Scene>> Scenes
        {
            get { return (SerializableDictionary<string, SerializableDictionary<int, Scene>>)this["Scenes"]; }
            set { this["Scenes"] = value; }
        }
    }
}
