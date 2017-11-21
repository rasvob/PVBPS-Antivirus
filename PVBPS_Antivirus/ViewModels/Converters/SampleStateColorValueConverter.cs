using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using AntiVirusLib.Models;

namespace PVBPS_Antivirus.ViewModels.Converters
{
    public class SampleStateColorValueConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FileModel model)
            {
                if (model.ScanTime > DateTime.MinValue)
                {
                    return model.IsClean ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Colors.Red);
                }
            }

            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}