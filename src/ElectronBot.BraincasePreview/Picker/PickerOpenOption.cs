using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI;

namespace ElectronBot.Braincase.Picker
{
    public class PickerOpenOption
    {
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;
        public Thickness Margin { get; set; } = new Thickness();
        public bool EnableTapBlackAreaExit { get; set; } = false;
        public Brush Background { get; set; } = new SolidColorBrush(Color.FromArgb(0x7f, 0x00, 0x00, 0x00));

        public TransitionCollection Transitions { get; set; } = new TransitionCollection
        {
            new EntranceThemeTransition()
        };
    }
}
