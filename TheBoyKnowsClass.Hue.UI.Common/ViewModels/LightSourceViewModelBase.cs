using TheBoyKnowsClass.Common.UI.ViewModels;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Interfaces;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public abstract class LightSourceViewModelBase<T> : ViewModelBase
        where T : ILightSource, new()
    {
        protected T LightSourceModel;

        protected LightSourceViewModelBase(T lightSourceModel)
        {
            LightSourceModel = lightSourceModel;
        }

        public abstract LightSourceType LightSourceType { get; }

        public abstract string ID { get; }

        public string Name
        {
            get { return LightSourceModel.Name; }
            set
            {
                LightSourceModel.SetNameAsync<T>(value).ContinueWith(x => RaisePropertyChanged("Name"));
            }
        }

    }
}
