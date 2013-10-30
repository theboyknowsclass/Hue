using System;
using TheBoyKnowsClass.Common.Models;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.UI.Common.Enumerations;
using TheBoyKnowsClass.Hue.UI.Common.Interfaces;

namespace TheBoyKnowsClass.Hue.UI.Common.Models
{
    public static class SettingsInitialiser
    {
        public static ISettings Initialise(string deviceType, ISettings settings)
        {
            if (settings.ApplicationID.Value == null)
            {
                settings.ApplicationID = GenerateApplicationID();
            }

            if (settings.DeviceType.Value == null)
            {
                settings.DeviceType = deviceType;
            }

            if (settings.Bridge.Value == null)
            {
                settings.Bridge = new Setting<string>();
            }

            settings.Save();

            return settings;
        }

        public static void InitialiseScenes(ISettings settings, Bridge bridge)
        {
            if (settings.Scenes == null)
            {
                settings.Scenes = new SerializableDictionary<string, SerializableDictionary<int, Scene>>(); 
            }

            if (!settings.Scenes.ContainsKey(bridge.InternalIPAddress))
            {
                var scenes = new SerializableDictionary<int, Scene>();

                var allOff = new Scene(bridge, false, false) { Name = "All Off", Description = "Switch Off All Lights", IconStyle = IconStyle.Resource, IconLocation = "AllOffCanvas", SceneBrightness = (int?)0, SortOrder = 0 };
                allOff.SceneStates.Add(new SceneState(LightSourceType.Group, SceneType.HSB, "1") { On = false });
                allOff.Categories.Add("All");
                var relax = new Scene(bridge, false, true) { Name = "Relax", Description = "Relaxing Lights", IconStyle = IconStyle.Resource, IconLocation = "RelaxCanvas", SceneBrightness = (int?)255, SortOrder = 1 };
                relax.SceneStates.Add(new SceneState(LightSourceType.Group, SceneType.ColourTemperature, "1") { ColorTemperature = 469, On = true });
                relax.Categories.Add("All");
                var reading = new Scene(bridge, false, true) { Name = "Reading", Description = "Reading Lights", IconStyle = IconStyle.Resource, IconLocation = "ReadingfCanvas", SceneBrightness = (int?)255, SortOrder = 2 };
                reading.SceneStates.Add(new SceneState(LightSourceType.Group, SceneType.ColourTemperature, "1") { ColorTemperature = 346, On = true });
                reading.Categories.Add("All");
                var concentration = new Scene(bridge, false, true) { Name = "Concentration", Description = "Concentration Lights", IconStyle = IconStyle.Resource, IconLocation = "ConcentrationCanvas", SceneBrightness = (int?)255, SortOrder = 3 };
                concentration.SceneStates.Add(new SceneState(LightSourceType.Group, SceneType.ColourTemperature, "1") { ColorTemperature = 233, On = true });
                concentration.Categories.Add("All");
                var energising = new Scene(bridge, false, true) { Name = "Energising", Description = "Energising Lights", IconStyle = IconStyle.Resource, IconLocation = "EnergisingCanvas", SceneBrightness = (int?)255, SortOrder = 4 };
                energising.SceneStates.Add(new SceneState(LightSourceType.Group, SceneType.ColourTemperature, "1") { ColorTemperature = 156, On = true });
                energising.Categories.Add("All");

                scenes.Add(allOff.SortOrder.Value, allOff);
                scenes.Add(relax.SortOrder.Value, relax);
                scenes.Add(reading.SortOrder.Value, reading);
                scenes.Add(concentration.SortOrder.Value, concentration);
                scenes.Add(energising.SortOrder.Value, energising);

#if DEBUG
                var testScene = new Scene(bridge, true, true) { Name = "Test CT", Description = "Test", IconLocation = "pack://application:,,,/TheBoyKnowsClass.Hue.UI.Common;Component/Resources/off.png", SceneBrightness = (int?)255 };
                testScene.SceneStates.Add(new SceneState(LightSourceType.Group, SceneType.ColourTemperature, "1") { ColorTemperature = 250, On = true });
                testScene.Categories.Add("All");
                testScene.Categories.Add("Test");
                scenes.Add(5, testScene);
                var testScene2 = new Scene(bridge, true, true) { Name = "Test HSB", Description = "Test", IconLocation = "pack://application:,,,/TheBoyKnowsClass.Hue.UI.Common;Component/Resources/off.png", SceneBrightness = (int?)255 };
                testScene2.SceneStates.Add(new SceneState(LightSourceType.Light, SceneType.HSB, "1") { Hue = 0, Saturation = 255, On = true });
                testScene2.SceneStates.Add(new SceneState(LightSourceType.Light, SceneType.HSB, "2") { Hue = 65280, Saturation = 255, On = true });
                testScene2.SceneStates.Add(new SceneState(LightSourceType.Light, SceneType.HSB, "3") { Hue = 65280, Saturation = 0, On = true });
                testScene2.SceneStates.Add(new SceneState(LightSourceType.Light, SceneType.HSB, "4") { Hue = 0, Saturation = 0, On = true });
                testScene2.SceneStates.Add(new SceneState(LightSourceType.Light, SceneType.HSB, "5") { Hue = 56100, Saturation = 150, On = true });
                testScene2.Categories.Add("All");
                testScene2.Categories.Add("Test");
                scenes.Add(6, testScene2);
#endif

                settings.Scenes.Add(bridge.InternalIPAddress,scenes);
            }
            settings.Save();
        }

        private static string GenerateApplicationID()
        {
            #if DEBUG
            return "newdeveloper1";
            #else
            return new Guid().ToString().Substring(0, 31);
            #endif
        }
    }
}
