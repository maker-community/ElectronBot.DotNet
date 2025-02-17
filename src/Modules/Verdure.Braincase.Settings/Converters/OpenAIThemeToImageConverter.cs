using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace Verdure.Braincase.Settings.Converters;
public class ThemeToImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var theme = value as string;
        if (theme == "Dark")
        {
            return new SvgImageSource(new Uri("ms-appx:///Assets/Providers/OpenAI-black-monoblossom.svg"));
        }
        else
        {
            return new SvgImageSource(new Uri("ms-appx:///Assets/Providers/OpenAI-white-monoblossom.svg"));
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
