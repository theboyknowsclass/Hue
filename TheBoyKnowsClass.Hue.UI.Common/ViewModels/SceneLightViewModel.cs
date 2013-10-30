using System;
using TheBoyKnowsClass.Hue.Common.Models;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public class SceneLightViewModel : SceneLightSourceViewModelBase<LightViewModel, Light>
    {
        public SceneLightViewModel(SceneViewModel sceneViewModel, LightViewModel lightSourceViewModel)
        : base(sceneViewModel, lightSourceViewModel)
        {
        }

        public override Type DataType
        {
            get { return typeof(SceneLightViewModel); }
        }
}
}
