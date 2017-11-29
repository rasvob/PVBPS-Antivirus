using System;
using System.Globalization;
using System.Windows.Data;

namespace PVBPS_Antivirus.ViewModels.Converters
{
    public class DetectedByValueConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                return $"Detected by: {s}";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}