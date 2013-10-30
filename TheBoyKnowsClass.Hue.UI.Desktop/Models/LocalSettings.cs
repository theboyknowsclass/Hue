using System.Configuration;
using TheBoyKnowsClass.Common.Models;
using TheBoyKnowsClass.Common.UI.WPF.Modern.Interfaces;
using TheBoyKnowsClass.Hue.UI.Common.Interfaces;
using TheBoyKnowsClass.Hue.UI.Common.Models;

namespace TheBoyKnowsClass.Hue.UI.Desktop.Models
{
    public class LocalSettings : ApplicationSettingsBase, ISettings, IAppearanceSettings
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
            get
            {
                if (this["Bridge"] == null)
                {
                    this["Bridge"] = new Setting<string>();
                } 
                return (Setting<string>)this["Bridge"];
            }
            set { this["Bridge"] = value; }
        }

        [UserScopedSetting]
        public Setting<string> DeviceType
        {
            get
            {
                if (this["DeviceType"] == null)
                {
                    this["DeviceType"] = new Setting<string>();
                }
                return (Setting<string>)this["DeviceType"];
            }
            set { this["DeviceType"] = value; }
        }

        [UserScopedSetting]
        public Setting<string> ApplicationID
        {
            get
            {
                if (this["ApplicationID"] == null)
                {
                    this["ApplicationID"] = new Setting<string>();    
                }

                return (Setting<string>)this["ApplicationID"];
            }
            set { this["ApplicationID"] = value; }
        }

        [UserScopedSetting]
        public SerializableDictionary<string,SerializableDictionary<int, Scene>> Scenes
        {
            get { return (SerializableDictionary<string, SerializableDictionary<int, Scene>>)this["Scenes"]; }
            set { this["Scenes"] = value; }
        }

        [UserScopedSetting]
        public string Theme
        {
            get { return (string)this["Theme"]; }
            set { this["Theme"] = value; }
        }

        [UserScopedSetting]
        public string Accent
        {
            get { return (string)this["Accent"]; }
            set { this["Accent"] = value; }
        }
    }
}
