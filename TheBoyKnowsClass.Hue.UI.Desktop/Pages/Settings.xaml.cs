using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Navigation;
using TheBoyKnowsClass.Common.UI.Enumerations;
using TheBoyKnowsClass.Common.UI.WPF.Modern.Interfaces;
using TheBoyKnowsClass.Common.UI.WPF.Modern.ViewModels;

namespace TheBoyKnowsClass.Hue.UI.Desktop.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page, IModernPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        public async void OnNavigating(NavigatingCancelEventArgs e)
        {
            if (ApplyButton.IsEnabled)
            {
                e.Cancel = true;

                var response = await MessageHandlerViewModel.Instance.GetResponseAsync("Apply Changes", "You have unsaved changes.  Do you wish to apply them now?", MessageBoxButtons.YesNoCancel);

                if (response != MessageHandlerResponse.Cancel)
                {
                    ButtonAutomationPeer peer = response == MessageHandlerResponse.Yes ? new ButtonAutomationPeer(ApplyButton) : new ButtonAutomationPeer(CancelButton);

                    var invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    if (invokeProvider != null)
                    {
                        invokeProvider.Invoke();
                    }
                    
                }
            }
        }
    }
}
