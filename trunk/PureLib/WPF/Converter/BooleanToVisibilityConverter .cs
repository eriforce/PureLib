using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

namespace PureLib.WPF {
    public class BooleanToVisibilityConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool isVisible = bool.Parse(value.ToString());
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
