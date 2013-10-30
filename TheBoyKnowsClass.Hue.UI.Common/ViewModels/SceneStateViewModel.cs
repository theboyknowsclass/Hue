using System;
using TheBoyKnowsClass.Common.UI.Interfaces;
using TheBoyKnowsClass.Common.UI.ViewModels;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.UI.Common.Enumerations;
using TheBoyKnowsClass.Hue.UI.Common.Helpers;
using TheBoyKnowsClass.Hue.UI.Common.Interfaces;
using TheBoyKnowsClass.Hue.UI.Common.Models;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public class SceneStateViewModel : ViewModelBase, IDragable, IDropTarget, IRedrawable, IEquatable<SceneStateViewModel>
    {
        private readonly SceneState _sceneState;
        private readonly SceneStatesViewModel _sceneStates;
        private LightSourceType _lightSourceType;
        private string _id;
        private double? _x;
        private double? _y;
        private int? _hue;
        private int? _mirek;
        private int? _saturation;
        private int? _brightness;

        public SceneStateViewModel(SceneState sceneState, SceneStatesViewModel sceneStates)
        {
            _sceneState = sceneState;
            _sceneStates = sceneStates;

            InitialiseSceneStateViewModel();
        }

        private void InitialiseSceneStateViewModel()
        {
            _lightSourceType = _sceneState.LightSourceType;
            _id = _sceneState.PointID;

            switch (_sceneState.ColorMode)
            {
                case "ct":
                    _mirek = _sceneState.ColorTemperature;
                    break;
                case "hs":
                    _hue = _sceneState.Hue;
                    break;
            }

            if (_sceneState.SceneType == SceneType.Image)
            {
                _sceneState.Point = new Point { X = _x, Y = _y };
            }

            _saturation = _sceneState.Saturation;
            _brightness = _sceneState.Brightness;

        }

        public bool HasChanged()
        {
            return _lightSourceType != _sceneState.LightSourceType ||
                   _id != _sceneState.PointID ||
                   _saturation != _sceneState.Saturation ||
                   _brightness != _sceneState.Brightness ||
                   (_sceneState.ColorMode == "ct" & _mirek != _sceneState.ColorTemperature) ||
                   (_sceneState.ColorMode == "hs" & _hue != _sceneState.Hue);
        }

        public void Save()
        {
            _sceneState.LightSourceType = _lightSourceType;
            _sceneState.PointID = _id;

            switch (_sceneState.ColorMode)
            {
                case "ct":
                    _sceneState.ColorTemperature = _mirek;
                    break;
                case "hs":
                    _sceneState.Hue = _hue;
                    break;
            }

            _sceneState.Saturation = _saturation;
            _sceneState.Brightness = _brightness;

            if (_sceneState.SceneType == SceneType.Image)
            {
                _sceneState.Point = new Point { X = _x, Y = _y };
            }
        }

        public SceneType SceneType { get { return _sceneState.SceneType; } }

        public LightSourceType LightSourceType
        {
            get { return _lightSourceType; }
            private set
            {
                if (_lightSourceType == value) return;
                _lightSourceType = value; 
                RaisePropertyChanged("LightSourceType");
            }
        }

        public string ID 
        {
            get { return _id; }
            private set
            {
                if (_id == value) return;
                _id = value; RaisePropertyChanged("ID");
            }
        }

        public double? X 
        { 
            get
            {
                if (_x == null)
                {
                    _x = _sceneState.Point == null ? GetPointX() : _sceneState.Point.X;
                }

                return _x ?? 0.5;
            }
            set
            {
                if (HasChanged(_x, value))
                {
                    _x = value;
                    SetPointX(value);
                    RaisePropertyChanged("X");
                }
            }
        }

        public double? Y
        {
            get
            {
                if (_y == null)
                {
                    _y = _sceneState.Point == null ? GetPointY() : _sceneState.Point.Y;
                }

                return _y ?? 0.5;
            }
            set
            {
                if (HasChanged(_y,value))
                {
                    _y = value;
                    SetPointY(value);
                    RaisePropertyChanged("Y");
                }
            }
        }

        public int? Hue
        {
            get { return _sceneState.ColorMode == "hs" ? _hue : null; }
        }

        public int? Mirek
        {
            get { return _sceneState.ColorMode == "ct" ? _mirek : null; }
        }

        public int? Kelvin
        {
            get { return ColourHelper.GetKelvinFromMirek(Mirek); }
        }

        public int? Saturation
        {
            get { return _saturation; }
        }

        public int? Brightness
        {
            get { return _brightness; }
        }

        public Colour Color
        {
            get
            {
                if (_sceneState != null)
                {
                    switch (_sceneState.ColorMode)
                    {
                        case "ct":
                            return ColourHelper.GetRGBFromMirekAndSaturation(Mirek, Saturation);
                        case "hs":
                            return ColourHelper.GetRGBFromXAndSaturation(X, Saturation);
                    }
                }
                return new Colour();
            }
        }

        #region Helper Methods

        private bool HasChanged(double? oldValue, double? newValue)
        {
            if (!newValue.HasValue)
            {
                return false;
            }

            if (!oldValue.HasValue)
            {
                return true;
            }

            return (Math.Abs(oldValue.Value - newValue.Value) > 0.001);
        }

        private double? GetPointX()
        {
            if (_sceneState != null)
            {
                switch (_sceneState.ColorMode)
                {
                    case "ct":
                        return ColourHelper.GetXFromMirek(_sceneState.ColorTemperature);
                    case "xy":
                        return _sceneState.CIEColor[0];
                    case "hs":
                        return ColourHelper.GetXFromHue(_sceneState.Hue);
                }
            }
            return null;
        }

        private double? GetPointY()
        {
            if (_sceneState != null)
            {
                switch (_sceneState.ColorMode)
                {
                    case "ct":
                        return ColourHelper.GetYFromSaturation(_sceneState.Saturation);
                    case "xy":
                        return _sceneState.CIEColor[1];
                    case "hs":
                        return ColourHelper.GetYFromSaturation(_sceneState.Saturation);
                }
            }
            return null;
        }

        private void SetPointX(double? value)
        {
            if (_sceneState != null)
            {
                switch (_sceneState.ColorMode)
                {
                    case "ct":
                        _mirek = ColourHelper.GetMirekFromX(value);
                        _sceneState.SetSaturationAndColourTemperature(_sceneStates.Connection, _saturation, _mirek);
                        RaisePropertyChanged("Mirek");
                        RaisePropertyChanged("Kelvin");
                        break;
                    case "xy":
                        if (value != null) _sceneState.CIEColor[0] = value.Value;
                        break;
                    case "hs":
                        _hue = ColourHelper.GetHueFromX(value);
                        _sceneState.SetSaturationAndHue(_sceneStates.Connection, _saturation, _hue);
                        RaisePropertyChanged("Hue");
                        break;
                }
                RaisePropertyChanged("Color");
            }
        }

        private void SetPointY(double? value)
        {
            if (_sceneState != null)
            {
                switch (_sceneState.ColorMode)
                {
                    case "ct":
                        _saturation = ColourHelper.GetSaturationFromY(value);
                        _sceneState.SetSaturationAndColourTemperature(_sceneStates.Connection, _saturation, _mirek);
                        RaisePropertyChanged("Saturation");
                        break;
                    case "xy":
                        if (value != null) _sceneState.CIEColor[1] = value.Value;
                        break;
                    case "hs":
                        _saturation = ColourHelper.GetSaturationFromY(value);
                        _sceneState.SetSaturationAndHue(_sceneStates.Connection, _saturation, _hue);
                        RaisePropertyChanged("Saturation");
                        break;
                }
                RaisePropertyChanged("Color");
            }
        }

        #endregion

        public bool Equals(SceneStateViewModel other)
        {
            return LightSourceType == other.LightSourceType && ID == other.ID;
        }

        public void ReDraw()
        {
            RaisePropertyChanged("X");
            RaisePropertyChanged("Y");
            RaisePropertyChanged("Color");
        }

        public void Drop(object data, int? index = null, double? x = null, double? y = null)
        {
            if (data == null) return;

            var sceneLightViewModel = data as SceneLightViewModel;
            if (sceneLightViewModel != null)
            {
                //// add light to collection
                //var state = new SceneState(sceneLightViewModel.LightSource.LightSourceType, _sceneType, sceneLightViewModel.LightSource.ID);
                //var stateViewModel = new SceneStateViewModel(state, this) { X = x, Y = y };
                //Add(stateViewModel);
                //sceneLightViewModel.RefreshState();
                return;
            }

            var sceneGroupViewModel = data as SceneGroupViewModel;
            if (sceneGroupViewModel != null)
            {
                //var state = new SceneState(sceneGroupViewModel.LightSource.LightSourceType, _sceneType, sceneGroupViewModel.LightSource.ID);
                //var stateViewModel = new SceneStateViewModel(state, this) { X = x, Y = y };
                //Add(stateViewModel);
                //sceneGroupViewModel.RefreshState();
                return;
            }

            return;
        }

        public Type DataType 
        {
            get { return typeof(SceneStateViewModel); }
        }
    }
}
