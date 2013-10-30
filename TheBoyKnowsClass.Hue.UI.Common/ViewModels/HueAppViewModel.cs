using System.Collections.ObjectModel;
using TheBoyKnowsClass.Common.UI.Interfaces;
using TheBoyKnowsClass.Common.UI.ViewModels;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;
using TheBoyKnowsClass.Hue.Common.Models.Factories;
using TheBoyKnowsClass.Hue.UI.Common.Interfaces;
using TheBoyKnowsClass.Hue.UI.Common.Models;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public abstract class HueAppViewModel : ViewModelBase
    {
        private readonly BridgeFinder _bridgeFinder;
        private ObservableCollection<BridgeViewModel> _bridges;

        private IMessageHandler _messageHandler;

        private readonly ISettings _settings;
        private readonly IDelegateCommandFactory _commandFactory;
        private readonly HueConnection _connection;
        private BridgeViewModel _selectedBridge;
        private ConnectedBridgeViewModel _firstBridgeViewModel;

        protected HueAppViewModel(ISettings settings, IDelegateCommandFactory commandFactory, IMessageHandler messageHandler)
        {
            _connection = new HueConnection {Throttle = true};
            _bridgeFinder = new BridgeFinder();
            Bridges = new ObservableCollection<BridgeViewModel>();

            _settings = settings;
            _commandFactory = commandFactory;
            MessageHandler = messageHandler;

            ScanBridgesCommand = _commandFactory.CreateCommand(ExecuteScanBridgesAsync, CanExecuteScanBridges);
            ExecuteScanBridgesAsync();

            SaveCommand = _commandFactory.CreateCommand(ExecuteSave, CanExecuteSave);

        }

        #region Public Properties

        public HueConnection Connection
        {
            get { return _connection; }
        }

        public ObservableCollection<BridgeViewModel> Bridges
        {
            get { return _bridges; }
            private set
            {
                _bridges = value;
                RaisePropertyChanged("Bridges");
            }
        }

        public BridgeViewModel FirstBridge
        {
            get { return _firstBridgeViewModel; }
        }

        public IMessageHandler MessageHandler
        {
            get { return _messageHandler; }
            set
            {
                _messageHandler = value;
                RaisePropertyChanged("MessageHandler");
            }
        }

        #region Settings 

        public BridgeViewModel SelectedBridge
        {
            get { return _selectedBridge; }
            set
            {
                if (_selectedBridge != value)
                {
                    if (value != null)
                    {
                        _selectedBridge = value;
                        if (!_selectedBridge.IsConnected)
                        {
                            _selectedBridge.ConnectAsync().ContinueWith(o => RaisePropertyChanged("SelectedBridge"));
                        }
                        RaisePropertyChanged("SelectedBridge");
                        RaisePropertyChanged("CanSave");
                    }
                }
            }
        }

        public string DeviceType
        {
            get { return _settings.DeviceType.Value; }
        }

        public string ApplicationID
        {
            get { return _settings.ApplicationID.Value; }
        }

        #endregion

        #endregion

        #region Commands

        #region Scan Bridges

        private bool _isScanningBridges;

        public IDelegateCommand ScanBridgesCommand { get; set; }

        private bool IsScanningBridges
        {
            get { return _isScanningBridges; }
            set
            {
                _isScanningBridges = value;
                RaisePropertyChanged("IsScanningOrConnecting");
            }
        }

        protected async void ExecuteScanBridgesAsync()
        {
            IsScanningBridges = true;
            ScanBridgesCommand.RaiseCanExecuteChanged();

            var rv = await _bridgeFinder.GetBridgesAsync(_connection);
            Bridges.Clear();
            _firstBridgeViewModel = null;
            if (!rv.IsError())
            {
                foreach (Bridge bridge in ((HueObjectCollectionBase<Bridge>) rv).Dictionary.Values)
                {
                    SettingsInitialiser.InitialiseScenes(_settings, bridge);
                    var bridgeViewModel = new BridgeViewModel(bridge, _settings, _commandFactory, _messageHandler);

                    if (_firstBridgeViewModel == null)
                    {
                        _firstBridgeViewModel = new ConnectedBridgeViewModel(bridge, _settings, _commandFactory, _messageHandler);
                        RaisePropertyChanged("FirstBridge");
                    }

                    Bridges.Add(bridgeViewModel);

                    if (_settings.Bridge.Value != null && _settings.Bridge.Value == bridge.InternalIPAddress)
                    {
                        SelectedBridge = bridgeViewModel;
                        //SelectedBridge.ConnectAsync().ContinueWith(o => RaisePropertyChanged("SelectedBridge"));
                    }
                }
            }
            else
            {
                Bridges = new ObservableCollection<BridgeViewModel>();
            }

            IsScanningBridges = false;
            ScanBridgesCommand.RaiseCanExecuteChanged();
        }

        protected bool CanExecuteScanBridges()
        {
            return !IsScanningBridges;
        }

        #endregion       

        #region Save Settings

        public IDelegateCommand SaveCommand { get; set; }

        private bool _isSavingSettings;

        private void ExecuteSave()
        {
            _isSavingSettings = true;
            SaveCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged("CanSave");

            _settings.Bridge = _selectedBridge.IPAddress;
            _settings.Save();
            if (!_selectedBridge.IsConnected)
            {
                _selectedBridge.ConnectAsync().ContinueWith(o => RaisePropertyChanged("SelectedBridge"));
            }
            _isSavingSettings = false;
            SaveCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged("CanSave");
            RaisePropertyChanged("IsConnected");
        }

        private bool CanExecuteSave()
        {
            return CanSave;
        }

        public bool CanSave
        {
            get
            {
                return !_isSavingSettings && _selectedBridge != null && _settings.Bridge.Value != _selectedBridge.IPAddress;
            }
        }

        #endregion

        #endregion
    }
}