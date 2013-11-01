using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TheBoyKnowsClass.Hue.UI.Common.Models;
using TheBoyKnowsClass.Hue.UI.Desktop.Helpers;

namespace TheBoyKnowsClass.Hue.UI.Desktop.Converters
{
    public class ColourToColorConverter : Freezable, IValueConverter
    {
        protected override Freezable CreateInstanceCore()
        {
            return new ColourToColorConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colour = value as Colour;
            return colour != null ? colour.ToColor() : new Color();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value is Color ? (Color) value : new Color();
            return color.ToColour();
        }
    }
}
