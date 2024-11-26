using Microsoft.UI.Xaml.Data;

namespace Verdure.Braincase.WinUI.Common.Converters;

public class UtcToLocalTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime utcDateTime)
        {
            var localDateTime = utcDateTime.ToLocalTime();
            var format = parameter as string;
            return string.IsNullOrEmpty(format) ? localDateTime.ToString() : localDateTime.ToString(format);
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string dateTimeString && DateTime.TryParse(dateTimeString, out var localDateTime))
        {
            return localDateTime.ToUniversalTime();
        }
        return value;
    }
}
