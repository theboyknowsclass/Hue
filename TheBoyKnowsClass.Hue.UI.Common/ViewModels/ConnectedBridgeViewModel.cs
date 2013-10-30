using TheBoyKnowsClass.Common.UI.Interfaces;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.UI.Common.Interfaces;

namespace TheBoyKnowsClass.Hue.UI.Common.ViewModels
{
    public class ConnectedBridgeViewModel : BridgeViewModel
    {
        public ConnectedBridgeViewModel(Bridge bridge, ISettings settings, IDelegateCommandFactory commandFactory, IMessageHandler messageHandler) : base(bridge, settings, commandFactory, messageHandler)
        {
            ConnectAsync();
        }

        new public bool IsConnected
        {
            get { return true; }
        }
    }
}
