using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TheBoyKnowsClass.Common.UI.WPF.Helpers;
using TheBoyKnowsClass.Hue.UI.Common.Enumerations;
using TheBoyKnowsClass.Hue.UI.Common.Interfaces;

namespace TheBoyKnowsClass.Hue.UI.Common.Desktop.Converters
{
    public class RatioToXYConverter : Freezable, IValueConverter
    {
        private Panel _itemsPanel;

        #region Dependency Property

        public FrameworkElement Parent
        {
            get { return (FrameworkElement)GetValue(ParentProperty); }
            set 
            { 
                SetValue(ParentProperty, value);
            }
        }

        public static readonly DependencyProperty ParentProperty = DependencyProperty.Register("Parent", typeof(FrameworkElement), typeof(RatioToXYConverter), new PropertyMetadata(null, OnParentPropertyChanged));

        private static void OnParentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var converter = d as RatioToXYConverter;
            if (converter != null)
            {
                converter.OnParentPropertyChanged(e);
            }
        }

        private void OnParentPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldValue = e.OldValue as FrameworkElement;
            if (oldValue != null)
            {
                oldValue.SizeChanged -= ValueOnSizeChanged;
            }

            var newValue = e.NewValue as FrameworkElement;
            if (newValue != null)
            {
                newValue.SizeChanged += ValueOnSizeChanged;
            }

            // = WPFHelper.GetVisualChild<StackPanel>(_itemsPresenter);

            OnDependencyPropertyChanged(this, e);
        }

        private void ValueOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (Data != null)
            {
                _itemsPanel = WPFHelper.GetVisualChild<Panel>(WPFHelper.GetVisualChild<ItemsPresenter>(Parent));
                Data.ReDraw();
            }
        }

        public Dimension ParentDimension
        {
            get { return (Dimension)GetValue(ParentDimensionProperty); }
            set { SetValue(ParentDimensionProperty, value); }
        }

        public static readonly DependencyProperty ParentDimensionProperty = DependencyProperty.Register("ParentDimension", typeof(Dimension), typeof(RatioToXYConverter), new PropertyMetadata(Dimension.Width ,OnDependencyPropertyChanged));

        public Double Offset
        {
            get { return (Double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(Double), typeof(RatioToXYConverter), new PropertyMetadata(0.0, OnDependencyPropertyChanged));

        public IRedrawable Data
        {
            get { return (IRedrawable)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(IRedrawable), typeof(RatioToXYConverter), new PropertyMetadata(null, OnDependencyPropertyChanged));

        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((RatioToXYConverter)d).Data != null)
            {
                ((RatioToXYConverter)d).Data.ReDraw();
            }
        }

        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ratio = value as double?;

            if (ratio.HasValue)
            {
                return Math.Round((ratio.Value * GetParentDimension()) + Offset);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = value as double?;
            if (x.HasValue)
            {
                return Math.Round((x.Value - Offset) / GetParentDimension(), 3);
            }
            return null;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new RatioToXYConverter();
        }

        private double GetParentDimension()
        {
            var parent = _itemsPanel ?? Parent;

            if (parent != null)
            {
                switch (ParentDimension)
                {
                    case Dimension.Height:
                        return parent.ActualHeight;
                    case Dimension.Width:
                        return parent.ActualWidth;
                }
            }
            return 0;
        }
    }
}
