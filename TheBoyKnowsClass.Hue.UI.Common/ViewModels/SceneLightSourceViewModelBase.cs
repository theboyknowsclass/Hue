using System;
using System.Collections.Generic;
using System.Linq;
using TheBoyKnowsClass.Common.UI.Interfaces;
using TheBoyKnowsClass.Common.UI.ViewModels;
using TheBoyKnowsClass.Hue.Common.Interfaces;
using TheBoyKnowsClass.Hue.UI.Common.Enumerations;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public abstract class SceneLightSourceViewModelBase<TA, TB> : ViewModelBase, IDragable
        where TA : LightSourceViewModelBase<TB>
        where TB : ILightSource, new()
    {
        private readonly SceneViewModel _sceneViewModel;
        private readonly TA _lightSourceViewModel;
        private bool _isInScene;

        protected SceneLightSourceViewModelBase(SceneViewModel sceneViewModel, TA lightSourceViewModel)
        {
            _sceneViewModel = sceneViewModel;
            _lightSourceViewModel = lightSourceViewModel;

            RefreshState();
        }

        public void RefreshState()
        {
            _isInScene = GetMatchingState().Any();
            RaisePropertyChanged("IsInScene");
        }

        #region Public Properties

        public bool IsInScene
        {
            get
            {
                return _isInScene;
            }
            set
            {
                if (_isInScene != value)
                {
                    if (value)
                    {
                        AddToScene();
                    }
                    else
                    {
                        RemoveFromScene();
                    }
                    _isInScene = value;
                    RaisePropertyChanged("IsInScene");
                }
            }
        }

        public TA LightSource
        {
            get { return _lightSourceViewModel; }
        }

        #endregion

        private void RemoveFromScene()
        {
            var existingState = GetMatchingState().FirstOrDefault();

            if (existingState == null) return;

            switch (existingState.SceneType)
            {
                case SceneType.HSB:
                    _sceneViewModel.HSBStates.Remove(existingState);
                    break;
                case SceneType.ColourTemperature:
                    _sceneViewModel.ColourTemperatureStates.Remove(existingState);
                    break;
                case SceneType.Image:
                    _sceneViewModel.ImageStates.Remove(existingState);
                    break;
            }
        }

        private void AddToScene()
        {
        }

        private IEnumerable<SceneStateViewModel> GetMatchingState()
        {
            return from s in _sceneViewModel.SceneStates
                   where s.LightSourceType == _lightSourceViewModel.LightSourceType && s.ID == _lightSourceViewModel.ID
                   select s;
        }

        public abstract Type DataType { get; }
    }
}
