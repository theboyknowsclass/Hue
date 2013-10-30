using System.Collections.ObjectModel;
using System.Linq;
using TheBoyKnowsClass.Common.UI.Interfaces;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.UI.Common.Enumerations;
using TheBoyKnowsClass.Hue.UI.Common.Interfaces;
using TheBoyKnowsClass.Hue.UI.Common.Models;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public class SceneStatesViewModel : ObservableCollection<SceneStateViewModel>, IDropTarget, IRedrawable
    {
        private readonly Scene _scene;
        private readonly SceneType _sceneType;
        private readonly HueConnection _connection;

        public SceneStatesViewModel(Scene scene, SceneType sceneType, HueConnection connection)
        {
            _scene = scene;
            _sceneType = sceneType;
            _connection = connection;

            foreach (var sceneState in from s in scene.SceneStates where s.SceneType == sceneType select s)
            {
                Add(new SceneStateViewModel(sceneState, this));
            }
        }

        public HueConnection Connection
        {
            get { return _connection; }
        }

        public void ReDraw()
        {
            foreach (SceneStateViewModel sceneStateViewModel in Items)
            {
                sceneStateViewModel.ReDraw();
            }
        }

        public void Drop(object data, int? index = null, double? x = null, double? y = null)
        {
            if (data == null) return;

            var sceneLightViewModel = data as SceneLightViewModel;
            if (sceneLightViewModel != null)
            {
                // add light to collection
                var state = new SceneState(sceneLightViewModel.LightSource.LightSourceType, _sceneType, sceneLightViewModel.LightSource.ID);
                var stateViewModel = new SceneStateViewModel(state, this) { X = x, Y = y };
                Add(stateViewModel);
                sceneLightViewModel.RefreshState();
                return;
            }

            var sceneGroupViewModel = data as SceneGroupViewModel;
            if (sceneGroupViewModel != null)
            {
                var state = new SceneState(sceneGroupViewModel.LightSource.LightSourceType, _sceneType, sceneGroupViewModel.LightSource.ID);
                var stateViewModel = new SceneStateViewModel(state, this) { X = x, Y = y };
                Add(stateViewModel);
                sceneGroupViewModel.RefreshState();
                return;
            }

            var sceneStateViewModel = data as SceneStateViewModel;
            if (sceneStateViewModel != null)
            {
                return;
            }

            return;
        }

        public bool HasChanged()
        {
            return string.Join(",",
                               from s in Items
                               where s.LightSourceType == LightSourceType.Light
                               orderby s.ID ascending
                               select s.ID) != string.Join(",", from s in _scene.SceneStates
                                                                where
                                                                    s.SceneType == _sceneType &&
                                                                    s.LightSourceType == LightSourceType.Light
                                                                orderby s.PointID
                                                                select s.PointID) ||
                               string.Join(",",
                               from s in Items
                               where s.LightSourceType == LightSourceType.Group
                               orderby s.ID ascending
                               select s.ID) != string.Join(",", from s in _scene.SceneStates
                                                                where
                                                                    s.SceneType == _sceneType &&
                                                                    s.LightSourceType == LightSourceType.Group
                                                                orderby s.PointID
                                                                select s.PointID);

        }
    }
}
