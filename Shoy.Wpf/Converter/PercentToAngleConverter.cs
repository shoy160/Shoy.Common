using System;
using System.Globalization;
using System.Windows.Data;

namespace Shoy.Wpf.Converter
{
    /// <summary> 百分比转换为角度值 </summary>
    public class PercentToAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double percent;
            if (!double.TryParse(value?.ToString() ?? "0", out percent))
                percent = 0;
            if (percent >= 1)
                return 360.0D;
            return percent * 360;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
