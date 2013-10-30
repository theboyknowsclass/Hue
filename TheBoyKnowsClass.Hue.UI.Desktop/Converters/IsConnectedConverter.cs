using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TheBoyKnowsClass.Hue.UI.Common.ViewModels;

namespace TheBoyKnowsClass.Hue.UI.Desktop.Converters
{
    public class IsConnectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var selectedBridge = value as BridgeViewModel;
            var visibleIfTrue = (bool)parameter;

            if (selectedBridge != null)
            {
                if (selectedBridge.IsConnected)
                {
                    return visibleIfTrue ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            return visibleIfTrue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
