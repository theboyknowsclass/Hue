using System;
using System.Diagnostics;
using Newtonsoft.Json;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.UI.Common.Enumerations;
using TheBoyKnowsClass.Hue.UI.Common.Helpers;

namespace TheBoyKnowsClass.Hue.UI.Common.Models
{
    public class SceneState : State
    {
        private SceneType _sceneType;

        public SceneState()
        {
            Point = null;
        }

        public SceneState(LightSourceType lightSourceType, SceneType sceneType, string id) : this()
        {
            LightSourceType = lightSourceType;
            SceneType = sceneType;
            PointID = id;
            ColorMode = ColourHelper.GetColorMode(SceneType);
        }

        [JsonIgnore]
        public LightSourceType LightSourceType { get; set; }
        [JsonIgnore]
        public SceneType SceneType
        {
            get { return _sceneType; }
            set 
            { 
                _sceneType = value;
                ColorMode = ColourHelper.GetColorMode(value);
            }
        }

        [JsonIgnore]
        public string PointID { get; set; }
        [JsonIgnore]
        public string ImageLocation { get; set; }
        [JsonIgnore]
        public Point Point { get; set; }

        public void SetState(HueConnection connection, int? brightness)
        {
            Brightness = brightness ?? Brightness;
            On = Brightness != 0;

            SetState(connection, this);
        }

        public void SetBrightness(HueConnection connection, int? brightness)
        {
            SetState(connection, new State { Brightness = brightness, On = brightness != 0 });
        }

        public void SetSaturationAndHue(HueConnection connection, int? saturation, int? hue)
        {
            SetState(connection, new State { Saturation = saturation, Hue = hue, On = true });
        }

        public void SetSaturationAndColourTemperature(HueConnection connection, int? saturation, int? colourTemperature)
        {
            SetState(connection, new State { Saturation = saturation, ColorTemperature = colourTemperature, On = true });
        }

        internal void SetState(HueConnection connection, State newState)
        {
            switch (LightSourceType)
            {
                case LightSourceType.Group:
                    //connection.Bridge.Groups.Dictionary[PointID].SetStateAsync(newState);
                    var bridge = connection.Bridge;
                    var groups = bridge.Groups;
                    var dict = groups.Dictionary;
                    var group = dict[PointID];
                    var rv = group.SetStateAsync(newState);
                    break;
                case LightSourceType.Light:
                    connection.Bridge.Lights.Dictionary[PointID].SetStateAsync(newState).ContinueWith(task => Debug.WriteLine(task.Result));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
