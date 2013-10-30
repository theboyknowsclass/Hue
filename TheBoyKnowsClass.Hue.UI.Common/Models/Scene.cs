using System.Collections.Generic;
using TheBoyKnowsClass.Common.Models;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.UI.Common.Enumerations;

namespace TheBoyKnowsClass.Hue.UI.Common.Models
{
    public class Scene
    {
        private readonly Bridge _bridge;

        public Scene() : this(null, true, true)
        {
        }

        public Scene(Bridge bridge) : this(bridge, true, true)
        {
        }

        public Scene(Bridge bridge, bool editable, bool brightnessEditable)
        {
            SortOrder = new Setting<int>();
            Name = new Setting<string>();
            Description = new Setting<string>();
            IconStyle = new Setting<IconStyle>();
            IconLocation = new Setting<string>();
            IndividualBrightness = new Setting<bool>();

            Categories = new List<string>();
            SceneStates = new List<SceneState>();

            _bridge = bridge;
            Editable = new Setting<bool>(editable);
            BrightnessEditable = new Setting<bool>(brightnessEditable);
        }

        public Setting<int> SortOrder { get; set; }
        public Setting<string> Name { get; set; }
        public Setting<string> Description { get; set; }
        public Setting<IconStyle> IconStyle { get; set; }
        public Setting<string> IconLocation { get; set; }
        public Setting<bool> Editable { get;  set; }
        public Setting<bool> BrightnessEditable { get; set; }
        public Setting<bool> IndividualBrightness { get; set; }
        public Setting<int?> SceneBrightness { get; set; }
        public List<string> Categories { get; set; }
        public List<SceneState> SceneStates { get; set; } 

        #region Helper Methods

        public void SetScene(HueConnection connection, int? brightness)
        {
            foreach (SceneState sceneState in SceneStates)
            {
                sceneState.SetState(connection, !IndividualBrightness.Value ? brightness : null);
            }
        }

        public void SetBrightness(HueConnection connection, int? brightness)
        {
            var newState = new State { Brightness = brightness, On = brightness != 0 };

            foreach (SceneState sceneState in SceneStates)
            {
                sceneState.SetState(connection, newState);
            }
        }

        #endregion
    }
}
