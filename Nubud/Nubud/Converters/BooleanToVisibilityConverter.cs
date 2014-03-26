using Microsoft.Phone.Controls;
using Nubud.Resources;
using Nubud.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Nubud.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool visibility = (bool)value;
            return visibility ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class VisibilityToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibility = (Visibility)value;
            return visibility == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class BitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return new BitmapImage(new Uri(value.ToString(), UriKind.Absolute))
            {
                CreateOptions = CreateOptions
            };
            //return CacheImageFileConverter.LoadImage(value.ToString());
        }

        public BitmapCreateOptions CreateOptions
        {
            get;
            set;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ThemeValueToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var theme = App.SettingsViewModel.Theme;
            if (object.Equals(parameter, Utils.DARK))
            {
                return theme == Utils.DARK;
            }
            else
            {
                return theme == Utils.LIGHT;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (object.Equals(parameter, Utils.DARK))
            {
                return parameter;
            }
            else
            {
                return Utils.LIGHT;
            }
        }
    }

    public class FontSizeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (object.Equals(parameter, "SMALL"))
            {
                return (int)value == Utils.SMALL;
            }
            else if (object.Equals(parameter, "MEDIUM"))
            {
                return (int)value == Utils.MEDIUM;
            }
            else
            {
                return (int)value == Utils.LARGE;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (object.Equals(parameter, "SMALL"))
            {
                return Utils.SMALL;
            }
            else if (object.Equals(parameter, "MEDIUM"))
            {
                return Utils.MEDIUM;
            }
            else
            {
                return Utils.LARGE;
            }
        }

        #endregion
    }
}
