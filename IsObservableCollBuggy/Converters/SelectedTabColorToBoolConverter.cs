using System;
using System.Globalization;
using Xamarin.Forms;

namespace IsObservableCollBuggy.Converters
{
    public class SelectedTabColorToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var selectedColor = (Color)value;

            if (selectedColor.Equals(Color.White)) return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
