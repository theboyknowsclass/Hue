using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using TheBoyKnowsClass.Common.UI.Interfaces;
using TheBoyKnowsClass.Common.UI.ViewModels;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.UI.Common.Enumerations;
using TheBoyKnowsClass.Hue.UI.Common.Models;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public class SceneViewModel : ViewModelBase, INotifyDataErrorInfo 
    {
        private readonly Scene _scene;
        private readonly HueConnection _connection;
        private readonly BridgeViewModel _bridgeViewModel;

        private string _name;
        private string _description;
        private IconStyle _iconStyle;
        private string _iconLocation;
        private int? _brightness;
        private SceneStatesViewModel _hsbStates;
        private SceneStatesViewModel _colourTemperatureStates;
        private SceneStatesViewModel _imageStates;
        private SceneStateViewModel _selectedHSBState;
        private SceneStateViewModel _selectedColourTemperatureState;
        private SceneStateViewModel _selectedImageState;
        private SceneGroupViewModel _selectedGroup;
        private SceneLightViewModel _selectedLight;
        private ObservableCollection<SceneLightViewModel> _lights;
        private ObservableCollection<SceneGroupViewModel> _groups;
        private ObservableCollection<string> _categories;
        private SceneType? _selectedSceneType;

        public SceneViewModel(Scene scene, BridgeViewModel bridgeViewModel, IDelegateCommandFactory commandFactory)
        {
            _scene = scene;
            _bridgeViewModel = bridgeViewModel;
            _bridgeViewModel.Lights.CollectionChanged += LightsOnCollectionChanged;
            _bridgeViewModel.Groups.CollectionChanged += GroupsOnCollectionChanged;
            _connection = bridgeViewModel.Context;

            SaveCommand = commandFactory.CreateCommand(ExecuteSave, CanExecuteSave);
            CancelCommand = commandFactory.CreateCommand(ExecuteCancel, CanExecuteCancel);

            InitialiseSceneViewModel(); 
        }

        private void InitialiseSceneViewModel()
        {
            Name = _scene.Name.Value;
            Description = _scene.Description.Value;
            IconStyle = _scene.IconStyle.Value;
            IconLocation = _scene.IconLocation.Value;
            Categories = new ObservableCollection<string>(_scene.Categories);
            _brightness = _scene.SceneBrightness.Value;
            _hsbStates = new SceneStatesViewModel(_scene, SceneType.HSB, _connection);
            _colourTemperatureStates = new SceneStatesViewModel(_scene, SceneType.ColourTemperature, _connection);
            _imageStates = new SceneStatesViewModel(_scene, SceneType.Image, _connection);
        }

        public bool HasChanged()
        {
            return Name != _scene.Name.Value ||
                   Description != _scene.Description.Value ||
                   IconLocation != _scene.IconLocation.Value ||
                   _brightness != _scene.SceneBrightness.Value ||
                   string.Join(",", (from c in Categories orderby c ascending select c)) !=
                   string.Join(",", (from c in _scene.Categories orderby c ascending select c)) ||
                   _hsbStates.HasChanged() ||
                   _colourTemperatureStates.HasChanged() ||
                   _imageStates.HasChanged();

        }

        public int SortOrder
        {
            get { return _scene.SortOrder.Value; }
            set { _scene.SortOrder.SetValue(value); }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value; RaisePropertyChanged("Name"); 
            }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged("Description"); }
        }

        public IconStyle IconStyle
        {
            get { return _iconStyle; }
            set
            {
                _iconStyle = value;
                RaisePropertyChanged("IconStyle");
            }
        }

        public string IconLocation
        {
            get { return _iconLocation; }
            set
            {
                _iconLocation = value;
                RaisePropertyChanged("IconLocation");
            }
        }

        public int? Brightness
        {
            get { return _brightness; }
            set 
            {
                if (value != _brightness)
                {
                    _brightness = value;
                    RaisePropertyChanged("Brightness");
                    SetBrightness();
                }
            }
        }

        public ObservableCollection<string> Categories
        {
            get { return _categories; }
            set 
            { 
                _categories = value;
                RaisePropertyChanged("Brightness");
            }
        }

        public bool IsEditable
        {
            get { return _scene.Editable.Value; }
        }

        public bool IsBrightnessEditable
        {
            get { return _scene.BrightnessEditable.Value; }
        }

        public SceneType? SelectedSceneType
        {
            get {
                return _selectedSceneType ?? (_selectedSceneType = (from s in _scene.SceneStates select s.SceneType).FirstOrDefault());
            }
            set { _selectedSceneType = value; RaisePropertyChanged("SelectedSceneType"); }
        }

        public SceneStatesViewModel HSBStates
        {
            get { return _hsbStates; }
        }

        public SceneStatesViewModel ColourTemperatureStates
        {
            get { return _colourTemperatureStates; }
        }

        public SceneStatesViewModel ImageStates
        {
            get { return _imageStates; }
        }

        public ObservableCollection<SceneStateViewModel> SceneStates
        {
            get { return new ObservableCollection<SceneStateViewModel>(_hsbStates.Union(_colourTemperatureStates).Union(_imageStates)); }
        }

        public SceneStateViewModel SelectedHSBState
        {
            get { return _selectedHSBState; }
            set 
            {

                _selectedHSBState = value;
                RaisePropertyChanged("SelectedHSBState");
                if (_selectedHSBState != null)
                {
                    SelectedColourTemperatureState = null;
                    SelectedImageState = null;
                }
            }
        }

        public SceneStateViewModel SelectedColourTemperatureState
        {
            get { return _selectedColourTemperatureState; }
            set
            {

                _selectedColourTemperatureState = value;
                RaisePropertyChanged("SelectedColourTemperatureState");
                RaisePropertyChanged("IsColourStateSelected");
                if (_selectedColourTemperatureState != null)
                {
                    SelectedHSBState = null;
                    SelectedImageState = null;
                }
            }
        }

        public bool IsColourStateSelected
        {
            get { return _selectedColourTemperatureState != null; }
        }

        public SceneStateViewModel SelectedImageState
        {
            get { return _selectedImageState; }
            set
            {

                _selectedImageState = value;
                RaisePropertyChanged("SelectedImageState");
                if (_selectedImageState != null)
                {
                    SelectedHSBState = null;
                    SelectedColourTemperatureState = null;
                }
            }
        }

        #region Lights and Groups

        public ObservableCollection<SceneLightViewModel> Lights
        {
            get 
            { 
                if (_lights == null)
                {
                    RefreshLights();

                }
                return _lights;
            }
            private set { _lights = value; }
        }

        private void RefreshLights()
        {
            Lights = new ObservableCollection<SceneLightViewModel>();

            foreach (var lightViewModel in _bridgeViewModel.Lights)
            {
                Lights.Add(new SceneLightViewModel(this, lightViewModel));
            } 
        }

        private void LightsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            RefreshLights();
        }

        public SceneLightViewModel SelectedLight
        {
            get { return _selectedLight; }
            set
            {
                if (_selectedLight != null)
                {
                    _selectedLight.LightSource.CancelAlertAsync().ContinueWith(x => { });
                }
                _selectedLight = value;
                RaisePropertyChanged("SelectedLight");

                if (_selectedLight != null)
                {
                    _selectedLight.LightSource.SetAlertCommand.Execute();
                }
            }
        }

        public ObservableCollection<SceneGroupViewModel> Groups
        {
            get
            {
                if (_groups == null)
                {
                    RefreshGroups();
                }
                return _groups;
            }
            private set { _groups = value; }
        }

        private void GroupsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            RefreshGroups();
        }

        private void RefreshGroups()
        {
            Groups = new ObservableCollection<SceneGroupViewModel>();

            foreach (var groupViewModel in _bridgeViewModel.Groups)
            {
                Groups.Add(new SceneGroupViewModel(this, groupViewModel));
            }
        }

        public SceneGroupViewModel SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                _selectedGroup = value;
                RaisePropertyChanged("SelectedGroup");
            }
        }


        #endregion

        #region Delegate Commands

        private bool _isCancelling;

        private bool CanExecuteCancel()
        {
            return HasChanged() && !_isCancelling;
        }

        private void ExecuteCancel()
        {
            if (!_isCancelling)
            {
                _isCancelling = true;
                CancelCommand.RaiseCanExecuteChanged();

                InitialiseSceneViewModel();

                _isCancelling = false;
                CancelCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isSaving;

        private bool CanExecuteSave()
        {
            return HasChanged() && !_isSaving && Error == null;
        }

        private void ExecuteSave()
        {
            if (!_isSaving)
            {
                _isSaving = true;
                SaveCommand.RaiseCanExecuteChanged();

                _scene.Name.SetValue(Name);
                _scene.Description.SetValue(Description);
                _scene.IconLocation.SetValue(IconLocation);
                _scene.Categories = Categories.ToList();
                _scene.SceneBrightness.SetValue(_brightness);

                foreach (SceneStateViewModel sceneStateViewModel in SceneStates)
                {
                    sceneStateViewModel.Save();
                }

                _isSaving = false;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public IDelegateCommand CancelCommand { get; set; }

        public IDelegateCommand SaveCommand { get; set; }

        #endregion

        #region helper functions

        public void SetScene()
        {
            _scene.SetScene(_connection, _brightness);
        }

        private void SetBrightness()
        {
            _scene.SetBrightness(_connection, _brightness);
        }

        #endregion

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "Name":
                        if (String.IsNullOrEmpty(Name))
                        {
                            return "Scene Name needs to be entered.";
                        }
                        break;
                }
                return null;
            }
        }

        public string Error 
        { 
            get { return this["Name"]; }
        }

        #region INotifyDataErrorInfo

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return this["Name"];
            }

            return this[propertyName];
        }

        public bool HasErrors 
        {
            get
            {
                return !string.IsNullOrEmpty(this["Name"]);
            }
        }

        #endregion
    }
}
