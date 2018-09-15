using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Helpers
{
    /// <summary>
    /// BooleanToVisibilityConverter class
    /// </summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Parameterless constructor of BooleanToVisibilityConverter class.
        /// </summary>
        public BooleanToVisibilityConverter()
        {
            //
        }

        /// <summary>
        /// Method which converts a Boolean value to a Visibility enumeration value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool && (bool)value)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        /// Method which converts a Visibility enumeration value to a Boolean value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Visibility && (Visibility)value == Visibility.Visible);
        }
    }
}
