using System;
using System.Threading.Tasks;
using TheBoyKnowsClass.Common.UI.Interfaces;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;
using TheBoyKnowsClass.Hue.Common.Models.Factories;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public class LightViewModel : LightSourceViewModelBase<Light>, IDragable
    {
        public LightViewModel(Light lightModel, IDelegateCommandFactory delegateCommandFactory) : base(lightModel)
        {
            SetAlertCommand = delegateCommandFactory.CreateCommand(SetAlertAsync, CanSetAlert);
        }

        public override LightSourceType LightSourceType
        {
            get { return LightSourceType.Light;}
        }

        public override string ID
        {
            get { return LightSourceModel.ID; }
        }

        public string Type
        {
            get { return LightSourceModel.Type; }
        }

        public string ModelID
        {
            get { return LightSourceModel.ModelID; }
        }

        public string SoftwareVersion
        {
            get { return LightSourceModel.SoftwareVersion; }
        }

        public bool IsAlerting { get; private set; }

        public async Task<HueObjectBase> CancelAlertAsync()
        {
            return await LightSourceModel.CancelAlertAsync();
        }

        #region Commands

        public IDelegateCommand SetAlertCommand { get; set; }

        private bool _isSettingAlert;

        private async void SetAlertAsync()
        {
            _isSettingAlert = true;
            SetAlertCommand.RaiseCanExecuteChanged();

            HueObjectBase rv = await LightSourceModel.SetAlertAsync();

            if (!rv.IsError())
            {
                bool refreshed = await LightSourceModel.RefreshAttributesAsync();

                if (refreshed)
                {
                    IsAlerting = LightSourceModel.State.Alert != "none";
                }
            }

            _isSettingAlert = false;
            SetAlertCommand.RaiseCanExecuteChanged();
        }

        private bool CanSetAlert()
        {
            return !_isSettingAlert;
        }

        #endregion

        public Type DataType { get { return typeof(LightViewModel); } }

        public override string ToString()
        {
            return Name;
        }
    }
}
