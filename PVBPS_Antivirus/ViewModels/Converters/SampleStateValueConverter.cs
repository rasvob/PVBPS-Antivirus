using System;
using System.Globalization;
using System.Windows.Data;
using AntiVirusLib.Models;

namespace PVBPS_Antivirus.ViewModels.Converters
{
    public class SampleStateValueConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FileModel model)
            {
                if (model.ScanTime > DateTime.MinValue)
                {
                    return model.IsClean ? " (Clean)" : " (Infected)";
                }
            }

            return " (???)";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}