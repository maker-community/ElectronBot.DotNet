using ElectronBot.DotNet;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ElectronBot.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        IElectronLowLevel electron = new ElectronLowLevel();
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void MyButton_Click(object sender, RoutedEventArgs e)
        {

            if (electron.Connect())
            {
                string assetsPath = Package.Current.InstalledLocation.Path + @"\Assets\frame.jpg";

                var mat = new OpenCvSharp.Mat(assetsPath, OpenCvSharp.ImreadModes.Color);

                var mat1 = mat.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Area);

                var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                var dataMeta = mat2.Data;

                var data = new byte[240 * 240 * 3];

                Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

                electron.SetImageSrc(data);

                electron.SetJointAngles(0, 0, 0, 0, 0, 0, true);

                electron.Sync();

                var list = electron.GetJointAngles();
            }
        }

        private void Head_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var head = Head.Value;

            var left = LeftArm.Value;

            var right = RightArm.Value;

            var bottom = Bottom.Value;

            electron.SetJointAngles((int)head, 0, (int)left, 0, (int)right, (int)bottom, true);

            electron.Sync();

            var list = electron.GetJointAngles();
        }
    }
}
