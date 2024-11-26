using System.Net.Mime;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace Verdure.Braincase.WinUI.Common.Converters;
public class Base64ToBitmapImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string base64String && !string.IsNullOrEmpty(base64String))
        {
            try
            {
                var bytes = System.Convert.FromBase64String(base64String.Replace($"data:{MediaTypeNames.Image.Png};base64,",""));
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes(bytes);
                        writer.StoreAsync().GetResults();
                    }

                    BitmapImage image = new BitmapImage();
                    image.SetSource(stream);
                    return image;
                }
            }
            catch
            {
                // Handle exceptions
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
