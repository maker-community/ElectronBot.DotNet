using Microsoft.UI.Xaml;
using Verdure.Braincase.Controls;

namespace Verdure.Braincase.Helpers;

public class ToastHelper
{
    public static void SendToast(string content, TimeSpan? duration = null)
    {
        var toast = new Toast(content);
        if (duration.HasValue)
        {
            toast.Duration = duration.Value;
        }
        toast.Show();
    }
    public static void SendFavoriteToast(string content, TimeSpan? duration = null)
    {
        var toast = new Toast(content);
        toast.Style = Application.Current.Resources["FavoriteToastStyle"] as Style;
        if (duration.HasValue)
        {
            toast.Duration = duration.Value;
        }
        toast.Show();
    }
}
