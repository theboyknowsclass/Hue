using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using TheBoyKnowsClass.Common.UI.Enumerations;
using TheBoyKnowsClass.Common.UI.Interfaces;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.Common.Models.Factories;
using TheBoyKnowsClass.Hue.UI.Common.Interfaces;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public class GroupViewModel : LightSourceViewModelBase<Group>, IDropTarget, IRedrawable, INotifyDataErrorInfo 
    {
        private readonly IMessageHandler _messageHandler;
        private readonly BridgeViewModel _bridge;
        private readonly ObservableCollection<GroupLightViewModel> _lights;
        private readonly List<string> _groupLightIDs;
        private string _name;

        public GroupViewModel(Group group, BridgeViewModel bridge, IDelegateCommandFactory commandFactory, IMessageHandler messageHandler) : base(group)
        {
            _messageHandler = messageHandler;
            _bridge = bridge;
            _lights = new ObservableCollection<GroupLightViewModel>();
            bridge.Lights.CollectionChanged += BridgeLightsOnCollectionChanged;
            _groupLightIDs = LightSourceModel.Lights.Dictionary.Keys.ToList();
            InitialiseGroupViewModel();

            SaveCommand = commandFactory.CreateCommand(ExecuteSaveAsync, CanExecuteSave);
            CancelCommand = commandFactory.CreateCommand(ExecuteCancel, CanExecuteCancel);
        }

        private void InitialiseGroupViewModel()
        {
            _name = LightSourceModel.Name;

            _lights.Clear();
            foreach (var groupLight in from l in _bridge.Lights select new GroupLightViewModel(this, l))
            {
                _lights.Add(groupLight);
            }
        }

        private void BridgeLightsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            ReDraw();
        }

        public override LightSourceType LightSourceType
        {
            get { return LightSourceType.Group; }
        }

        public override string ID
        {
            get { return LightSourceModel.ID; }
        }

        public string EditableName
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged("EditableName");
                RaisePropertyChanged("CanSave");
                RaisePropertyChanged("CanCancel");
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public ObservableCollection<GroupLightViewModel> Lights
        {
            get
            {
                return _lights;
            }
        }

        public bool IsEditable
        {
            get
            {
                int id;

                if (int.TryParse(ID, out id))
                {
                    if (id <= 1)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void ReDraw()
        {
            InitialiseGroupViewModel();
        }

        private bool HasChanged()
        {
            return Name != EditableName || string.Join(",", LightSourceModel.LightsIDs.ToArray()) != string.Join(",", _groupLightIDs.ToArray());
        }

        public async void Drop(object data, int? index = null, double? x = null, double? y = null)
        {
            if (!IsEditable)
            {
                await _messageHandler.GetResponseAsync("Error", "Lights cannot be added to this group", MessageBoxButtons.OK);
                return;
            }

            if (data is LightViewModel)
            {
                var light = data as LightViewModel;

                var rv = await LightSourceModel.AddLightByID(light.ID);

                if (rv.IsError())
                {
                    await _messageHandler.GetResponseAsync("Error", ((Error)rv).Description, MessageBoxButtons.OK);
                }
                else
                {
                    RaisePropertyChanged("Lights");
                }
            }

            if (data is GroupViewModel)
            {
                var group = data as GroupViewModel;
                RaisePropertyChanged("Lights");
            }
        }

        public bool ContainsLight(string id)
        {
            return _groupLightIDs.Contains(id);
        }

        public void AddLightDelayed(string id)
        {
            if (!_groupLightIDs.Contains(id))
            {
                _groupLightIDs.Add(id);
                RaisePropertyChanged("CanSave");
                RaisePropertyChanged("CanCancel");
                RaisePropertyChanged("Lights");
            }
        }

        public void RemoveLightDelayed(string id)
        {
            if (_groupLightIDs.Contains(id))
            {
                _groupLightIDs.Remove(id);
                RaisePropertyChanged("CanSave");
                RaisePropertyChanged("CanCancel");
                RaisePropertyChanged("Lights");
            } 
        }

        #region Delegate Commands

        public IDelegateCommand CancelCommand { get; set; }

        private bool _isCancelling;

        public bool CanCancel
        {
            get
            {
                return HasChanged() && !_isCancelling; 
            }
            
        }

        private bool CanExecuteCancel()
        {
            return CanCancel;
        }

        private void ExecuteCancel()
        {
            if (!_isCancelling)
            {
                _isCancelling = true;
                CancelCommand.RaiseCanExecuteChanged();

                if (string.IsNullOrEmpty(ID))
                {
                    _bridge.SelectedGroup = null;
                }
                else
                {
                    InitialiseGroupViewModel();
                }

                _isCancelling = false;
                CancelCommand.RaiseCanExecuteChanged();
            }
        }

        public IDelegateCommand SaveCommand { get; set; }

        private bool _isSaving;

        public bool CanSave
        {
            get
            {
                return HasChanged() && !_isSaving && Error == null;
            }
        }

        private bool CanExecuteSave()
        {
            return CanSave;
        }

        private async void ExecuteSaveAsync()
        {
            if (!_isSaving)
            {
                _isSaving = true;
                SaveCommand.RaiseCanExecuteChanged();

                if (string.IsNullOrEmpty(ID))
                {
                    await _bridge.CreateGroup(EditableName, _groupLightIDs);
                }
                else
                {
                    LightSourceModel.Name = EditableName;
                    LightSourceModel.LightsIDs = _groupLightIDs;
                    await LightSourceModel.SetAttributesAsync();
                }

                _isSaving = false;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region INotifyDataErrorInfo

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "EditableName":
                        if (String.IsNullOrEmpty(EditableName))
                        {
                            return "Group Name needs to be entered.";
                        }
                        break;
                    case "Lights":
                        if (_groupLightIDs.Count == 0)
                        {
                            return "One or more lights must be selected";
                        }
                        break;
                }
                return null;
            }
        }

        public string Error
        {
            get { return this["EditableName"]; }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return this["EditableName"];
            }

            return this[propertyName];
        }

        public bool HasErrors
        {
            get
            {
                return !string.IsNullOrEmpty(this["EditableName"]) ||
                    !string.IsNullOrEmpty(this["Lights"]);
            }
        }

        #endregion
    }
}
