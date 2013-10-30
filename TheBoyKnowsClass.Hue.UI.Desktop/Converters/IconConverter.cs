using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using TheBoyKnowsClass.Hue.UI.Common.Enumerations;
using TheBoyKnowsClass.Hue.UI.Common.ViewModels;

namespace TheBoyKnowsClass.Hue.UI.Desktop.Converters
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var scene = value as SceneViewModel;

            if (scene != null)
            {
                switch (scene.IconStyle)
                {
                    case IconStyle.Image:
                        return new Image {Source = new BitmapImage(new Uri(scene.IconLocation)){DecodePixelHeight = 76}};
                    case IconStyle.Resource:
                        if (Application.Current.Resources.Contains(scene.IconLocation))
                        {
                            return Application.Current.Resources[scene.IconLocation];
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
