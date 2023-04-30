using ElectronBot.Braincase.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ElectronBot.Braincase.Helpers
{
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
            toast.Style = App.Current.Resources["FavoriteToastStyle"] as Style;
            if (duration.HasValue)
            {
                toast.Duration = duration.Value;
            }
            toast.Show();
        }
    }
}
