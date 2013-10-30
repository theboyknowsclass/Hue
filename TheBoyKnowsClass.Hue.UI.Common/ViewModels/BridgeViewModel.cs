using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheBoyKnowsClass.Common.UI.Enumerations;
using TheBoyKnowsClass.Common.UI.Interfaces;
using TheBoyKnowsClass.Common.UI.ViewModels;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;
using TheBoyKnowsClass.Hue.Common.Models.Factories;
using TheBoyKnowsClass.Hue.UI.Common.Interfaces;
using TheBoyKnowsClass.Hue.UI.Common.Models;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public class BridgeViewModel : ViewModelBase
    {
        private readonly Bridge _bridge;
        private readonly ISettings _settings;
        private readonly IDelegateCommandFactory _commandFactory;
        private readonly IMessageHandler _messageHandler;

        private ObservableCollection<LightViewModel> _lights;
        private LightViewModel _selectedLight;

        private ObservableCollection<GroupViewModel> _groups;
        private GroupViewModel _selectedGroup;

        private ObservableCollection<SceneViewModel> _scenes;
        private SceneViewModel _selectedScene;
        private ObservableCollection<string> _sceneCategories;
        private string _selectedCategory;

        private readonly SynchronizationContext _context = SynchronizationContext.Current;

        public BridgeViewModel(Bridge bridge, ISettings settings, IDelegateCommandFactory commandFactory, IMessageHandler messageHandler)
        {
            _bridge = bridge;
            _settings = settings;
            _commandFactory = commandFactory;
            _messageHandler = messageHandler;

            Lights = new ObservableCollection<LightViewModel>();
            Groups = new ObservableCollection<GroupViewModel>();
            Scenes = new ObservableCollection<SceneViewModel>();

            ConnectCommand = _commandFactory.CreateCommand(ExecuteConnectAsync, CanExecuteConnect);
            GetLightsCommand = _commandFactory.CreateCommand(ExecuteGetLightsAsync, CanExecuteGetLights);
            GetGroupsCommand = _commandFactory.CreateCommand(ExecuteGetGroupsAsync, CanExecuteGetGroups);
            AddGroupCommand = _commandFactory.CreateCommand(ExecuteAddGroup, CanExecuteAddGroup);
            DeleteGroupCommand = _commandFactory.CreateCommand(ExecuteDeleteGroupAsync, CanExecuteDeleteGroup);

            SelectedScene = null;

            AddSceneCommand = _commandFactory.CreateCommand(ExecuteAddScene, CanExecuteAddScene);
            DeleteSceneCommand = _commandFactory.CreateCommand(ExecuteDeleteScene, CanExecuteDeleteScene);

            //initialise settings
            if (settings.Scenes != null)
            {
                foreach (KeyValuePair<int, Scene> sceneKVP in settings.Scenes[_bridge.InternalIPAddress])
                {
                    Scenes.Add(new SceneViewModel(sceneKVP.Value, this, commandFactory));
                }

                SceneCategories = new ObservableCollection<string>(settings.Scenes[_bridge.InternalIPAddress].Values.SelectMany(s => s.Categories.ToList()).Distinct());
            }
        }

        internal HueConnection Context
        {
            get
            {
                return _bridge.Context;
            }
        }

        public string ID
        {
            get { return _bridge.ID; }
        }

        public string MACAddress
        {
            get { return _bridge.MACAddress; }
        }

        public string IPAddress
        {
            get { return _bridge.InternalIPAddress; }
        }

        public bool IsConnected
        {
            get { return _bridge.IsLoggedIn; }
        }

        public LightViewModel SelectedLight
        {
            get { return _selectedLight; }
            set
            {
                if (_selectedLight != null)
                {
                    _selectedLight.CancelAlertAsync().ContinueWith(x => { });
                }
                _selectedLight = value;
                RaisePropertyChanged("SelectedLight");

                if (_selectedLight != null)
                {
                    _selectedLight.SetAlertCommand.Execute();
                }
            }
        }

        public ObservableCollection<LightViewModel> Lights
        {
            get { return _lights; }
            private set
            {
                _lights = value;
                RaisePropertyChanged("Lights");
            }
        }

        public GroupViewModel SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                _selectedGroup = value;
                RaisePropertyChanged("SelectedGroup");
                RaisePropertyChanged("CanEditGroup");
                DeleteGroupCommand.RaiseCanExecuteChanged();
            }
        }

        public GroupViewModel FirstGroup
        {
            get { return Groups.FirstOrDefault(); }
        }

        public ObservableCollection<GroupViewModel> Groups
        {
            get { return _groups; }
            private set
            {
                _groups = value;
                RaisePropertyChanged("Groups");
                RaisePropertyChanged("CanEditGroup");
                RaisePropertyChanged("CanDeleteGroup");
            }
        }

        public SceneViewModel FirstScene
        {
            get { return Scenes.FirstOrDefault(); }
        }

        public SceneViewModel SelectedScene
        {
            get { return _selectedScene; }
            set
            {
                if (value != null)
                {
                    _selectedScene = value;
                    RaisePropertyChanged("SelectedScene");
                    RaisePropertyChanged("IsSceneSelected");
                    RaisePropertyChanged("CanEditScene");
                    RaisePropertyChanged("CanDeleteScene");
                    if (_selectedScene != null)
                    {
                        _selectedScene.SetScene();
                    }
                }
            }
        }

        public bool IsSceneSelected
        {
            get { return SelectedScene != null; }
        }

        public bool CanEditScene
        {
            get { return IsSceneSelected && _selectedScene.IsEditable; }
        }

        public ObservableCollection<SceneViewModel> Scenes
        {
            get { return _scenes; }
            private set
            {
                _scenes = value;
                RaisePropertyChanged("Scenes");
            }
        }

        public ObservableCollection<string> SceneCategories
        {
            get { return _sceneCategories; }
            set
            {
                _sceneCategories = value;
                RaisePropertyChanged("SceneCategories");
            }
        }

        public string SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged("SelectedCategory");
                RaisePropertyChanged("Scenes");
            }
        }

        #region Add Scene Command

        public IDelegateCommand AddSceneCommand { get; set; }

        private bool _isAddingScene;

        private bool IsAddingScene
        {
            get { return _isAddingScene; }
            set
            {
                _isAddingScene = value;
                RaisePropertyChanged("IsAddingScene");
            }
        }

        private void ExecuteAddScene()
        {
            IsAddingScene = true;
            AddSceneCommand.RaiseCanExecuteChanged();
            var sceneViewModel = new SceneViewModel(new Scene(_bridge), this, _commandFactory);
            Scenes.Add(sceneViewModel);
            SelectedScene = sceneViewModel;
            IsAddingScene = false;
            AddSceneCommand.RaiseCanExecuteChanged();
        }

        public bool CanAddScene
        {
            get { return !IsAddingScene; }
        }

        private bool CanExecuteAddScene()
        {
            return CanAddScene;
        }

        #endregion

        #region Delete Scene Command

        public IDelegateCommand DeleteSceneCommand { get; set; }

        private bool _isDeletingScene;

        private bool IsDeletingScene
        {
            get { return _isDeletingScene; }
            set
            {
                _isDeletingScene = value;
                RaisePropertyChanged("IsDeletingScene");
            }
        }

        private void ExecuteDeleteScene()
        {
            IsDeletingScene = true;
            DeleteSceneCommand.RaiseCanExecuteChanged();
            Scenes.Remove(SelectedScene);
            _settings.Scenes[_bridge.InternalIPAddress].Remove(SelectedScene.SortOrder);
            IsDeletingScene = false;
            DeleteSceneCommand.RaiseCanExecuteChanged();
        }

        public bool CanDeleteScene
        {
            get { return !IsDeletingScene && IsSceneSelected && SelectedScene.IsEditable; }
        }

        private bool CanExecuteDeleteScene()
        {
            return CanAddScene;
        }

        #endregion

        private async Task<HueObjectBase> LoginAsync(string userID)
        {
            return await _bridge.LoginAsync(userID);
        }

        private async Task<HueObjectBase> CreateNewClientAsync(string clientType, string id)
        {
            return await _bridge.CreateNewClientAsync(clientType, id);
        }

        #region ConnectAsync Command

        public IDelegateCommand ConnectCommand { get; set; }

        private bool _isConnecting;

        private bool IsConnecting
        {
            get { return _isConnecting; }
            set
            {
                _isConnecting = value;
                RaisePropertyChanged("IsConnecting");
            }
        }

        public async void ExecuteConnectAsync()
        {
            await ConnectAsync();
        }

        public async Task<bool> ConnectAsync()
        {
            IsConnecting = true;
            ConnectCommand.RaiseCanExecuteChanged();

            _bridge.Connect();

            HueObjectBase rv = await LoginAsync(_settings.ApplicationID.Value);

            if (rv.IsError())
            {
                EventHandler eventHandler = async (s, e) => await TimerCallbackAsync();
                await _messageHandler.GetResponseAsync("Information", string.Format("Please press the Link button on your bridge."), 500, 30000, MessageBoxButtons.None, eventHandler);
                return false;
            }

            ExecuteGetLightsAsync();
            ExecuteGetGroupsAsync();
            IsConnecting = false;
            RaisePropertyChanged("IsConnected");
            ConnectCommand.RaiseCanExecuteChanged();
            return true;
        }

        private async Task TimerCallbackAsync()
        {
            var rv = await CreateNewClientAsync(_settings.DeviceType, _settings.ApplicationID);

            if (rv.IsError())
            {
                Error error = ((HueObjectCollectionBase<Error>)rv).Dictionary["0"];

                if (error.Type == 101)
                {
                }
                else
                {
                    // some other error occured;
                    _messageHandler.HideMessageBox();
                }
            }
            else
            {
                _messageHandler.HideMessageBox();
                await LoginAsync(_settings.ApplicationID);

                _context.Post(o => ExecuteGetLightsAsync(), this);
                _context.Post(o => ExecuteGetGroupsAsync(), this);
                IsConnecting = false;
                RaisePropertyChanged("IsConnected");
                ConnectCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanExecuteConnect()
        {
            return !IsConnecting && !IsConnected;
        }

        #endregion

        #region Get Lights Command

        public IDelegateCommand GetLightsCommand { get; set; }

        private bool _isGettingLights;


        private async void ExecuteGetLightsAsync()
        {
            _isGettingLights = true;
            GetLightsCommand.RaiseCanExecuteChanged();

            var rv = await _bridge.GetLightsFullAsync();

            Lights.Clear();

            if (!rv.IsError())
            {
                foreach (Light light in (from l in ((HueObjectCollectionBase<Light>)rv).Dictionary.Values orderby l.Name select l))
                {
                    Lights.Add(new LightViewModel(light, _commandFactory));
                }
            }
            else
            {
                await _messageHandler.GetResponseAsync("Error", "No Lights Found", MessageBoxButtons.OK);
            }

            _isGettingLights = false;
            GetLightsCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteGetLights()
        {
            return _bridge.IsLoggedIn && !_isGettingLights;
        }

        #endregion

        #region Get Groups Command

        public IDelegateCommand GetGroupsCommand { get; set; }

        private bool _isGettingGroups;

        private async void ExecuteGetGroupsAsync()
        {
            _isGettingGroups = true;
            GetGroupsCommand.RaiseCanExecuteChanged();

            var rv = await _bridge.GetGroupsFullAsync();

            Groups.Clear();

            if (!rv.IsError())
            {
                foreach (Group group in ((HueObjectCollectionBase<Group>)rv).Dictionary.Values)
                {
                    Groups.Add(new GroupViewModel(group, this, _commandFactory, _messageHandler));
                }
            }
            else
            {
                await _messageHandler.GetResponseAsync("Error", "No Groups Found", MessageBoxButtons.OK);
            }

            _isGettingLights = false;
            GetGroupsCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteGetGroups()
        {
            return _bridge.IsLoggedIn && !_isGettingGroups;
        }

        #endregion

        #region Add Group Command

        public IDelegateCommand AddGroupCommand { get; set; }

        private bool _isAddingGroup;

        private bool IsAddingGroup
        {
            get { return _isAddingGroup; }
            set
            {
                _isAddingGroup = value;
                RaisePropertyChanged("IsAddingGroup");
            }
        }

        private void ExecuteAddGroup()
        {
            IsAddingGroup = true;
            AddGroupCommand.RaiseCanExecuteChanged();

            var newGroup = new GroupViewModel(new Group(), this, _commandFactory, _messageHandler);
            SelectedGroup = newGroup;

            IsAddingGroup = false;
            AddGroupCommand.RaiseCanExecuteChanged();

        }

        public bool CanAddGroup
        {
            get { return !IsAddingGroup; }
        }

        private bool CanExecuteAddGroup()
        {
            return CanAddGroup;
        }

        public async Task CreateGroup(string groupName, IEnumerable<String> lightIDs)
        {

            var rv = await _bridge.CreateGroupAsync(groupName, lightIDs);

            if (rv.IsError())
            {
                HueObjectCollectionBase<Error> errors = rv.To<Error>();

                await _messageHandler.GetResponseAsync("Error", errors.FirstOrDefault().Description, MessageBoxButtons.OK);
            }
            else
            {
                SelectedGroup = null;
                ExecuteGetGroupsAsync();
            }
            
        }

        #endregion

        #region Delete Group Command

        public IDelegateCommand DeleteGroupCommand { get; set; }

        private bool _isDeletingGroup;

        private bool IsDeletingGroup
        {
            get { return _isDeletingGroup; }
            set
            {
                _isDeletingGroup = value;
                RaisePropertyChanged("IsDeletingGroup");
            }
        }

        private async void ExecuteDeleteGroupAsync()
        {
            IsDeletingGroup = true;
            DeleteGroupCommand.RaiseCanExecuteChanged();

            var rv = await _bridge.DeleteGroupAsync(SelectedGroup.ID);

            if (rv.IsError())
            {
                HueObjectCollectionBase<Error> errors = rv.To<Error>();

                await _messageHandler.GetResponseAsync("Error", errors.FirstOrDefault().Description, MessageBoxButtons.OK);
            }
            else
            {
                ExecuteGetGroupsAsync();
            }

            IsDeletingGroup = false;
            DeleteGroupCommand.RaiseCanExecuteChanged();

        }

        public bool CanEditGroup
        {
            get { return !IsDeletingGroup && SelectedGroup != null && SelectedGroup.IsEditable; }
        }

        private bool CanExecuteDeleteGroup()
        {
            return CanEditGroup;
        }

        #endregion
    }
}
