using System;
using TheBoyKnowsClass.Common.UI.Interfaces;
using TheBoyKnowsClass.Common.UI.ViewModels;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public class GroupLightViewModel : ViewModelBase, IDragable
    {
        private readonly GroupViewModel _groupViewModel;
        private readonly LightViewModel _lightViewModel;

        public GroupLightViewModel(GroupViewModel groupViewModel, LightViewModel lightViewModel)
        {
            _groupViewModel = groupViewModel;
            _lightViewModel = lightViewModel;
        }

        public LightViewModel Light
        {
            get
            {
                return _lightViewModel;
            }
        }

        public bool IsInGroup
        {
            get
            {
                return _groupViewModel.ContainsLight(_lightViewModel.ID);
            }
            set
            {
                if (_groupViewModel.ContainsLight(_lightViewModel.ID) != value)
                {
                    if (value)
                    {
                        AddToGroup();
                    }
                    else
                    {
                        RemoveFromGroup();
                    }

                    RaisePropertyChanged("IsInGroup");
                }
            }
        }

        private void RemoveFromGroup()
        {
            _groupViewModel.RemoveLightDelayed(_lightViewModel.ID);
        }

        private void AddToGroup()
        {
            _groupViewModel.AddLightDelayed(_lightViewModel.ID);
        }

        public Type DataType
        {
            get
            {
                return typeof (GroupLightViewModel);
            }
        }
    }
}
