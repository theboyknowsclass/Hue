using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TheBoyKnowsClass.Hue.UI.Common.Interfaces;
using TheBoyKnowsClass.Hue.UI.Common.Models;
using TheBoyKnowsClass.Hue.UI.Desktop.Helpers;
using Point = System.Windows.Point;

namespace TheBoyKnowsClass.Hue.UI.Desktop.Converters
{
    public class ColourMappingToLinearBrushConverter : Freezable, IValueConverter
    {
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register("StartPoint", typeof(Point), typeof(ColourMappingToLinearBrushConverter), new PropertyMetadata(new Point(0,0), OnDependencyPropertyChanged));

        public Point EndPoint
        {
            get { return (Point)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register("EndPoint", typeof(Point), typeof(ColourMappingToLinearBrushConverter), new PropertyMetadata(new Point(1, 0), OnDependencyPropertyChanged));

        public IRedrawable Data
        {
            get { return (IRedrawable)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(IRedrawable), typeof(ColourMappingToLinearBrushConverter), new PropertyMetadata(null, OnDependencyPropertyChanged));

        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((ColourMappingToLinearBrushConverter)d).Data != null)
            {
                ((ColourMappingToLinearBrushConverter)d).Data.ReDraw();
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var myCollection = value as Collection<ColourPointValueMapping>;

            if (myCollection != null)
            {
                var brush = new LinearGradientBrush { StartPoint = StartPoint, EndPoint = EndPoint };
                foreach (ColourPointValueMapping colourPointValueMapping in myCollection)
                {
                    brush.GradientStops.Add(new GradientStop(colourPointValueMapping.Colour.ToColor(), colourPointValueMapping.Point));
                }

                return brush;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected override Freezable CreateInstanceCore()
        {
            return new ColourMappingToLinearBrushConverter();
        }
    }
}
