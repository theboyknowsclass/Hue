using System;
using TheBoyKnowsClass.Hue.Common.Models;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public class SceneGroupViewModel : SceneLightSourceViewModelBase<GroupViewModel, Group>
    {
        public SceneGroupViewModel(SceneViewModel sceneViewModel, GroupViewModel lightSourceViewModel)
            : base(sceneViewModel, lightSourceViewModel)
        {
        }

        public override Type DataType
        {
            get { return typeof (SceneGroupViewModel); }
        }
    }
}
