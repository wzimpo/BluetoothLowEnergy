using System;
using System.Globalization;
using Xamarin.Forms;
namespace BluetoothLowEnergy.Util
{
    public class InvertBooleanConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return !(Boolean)value;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return !(Boolean)value;
        }
    }
}
