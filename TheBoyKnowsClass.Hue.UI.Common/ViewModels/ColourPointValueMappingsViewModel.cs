using System.Collections.ObjectModel;
using System.Collections.Specialized;
using TheBoyKnowsClass.Hue.UI.Common.Interfaces;
using TheBoyKnowsClass.Hue.UI.Common.Models;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public class ColourPointValueMappingsViewModel : ObservableCollection<ColourPointValueMapping>, IRedrawable
    {
        public void ReDraw()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
