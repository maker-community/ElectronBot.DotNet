using ElectronBot.DotNet;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {

            if (electron.Connect())
            {
                string assetsPath = Package.Current.InstalledLocation.Path + @"\Assets\yay.jpg";

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

        private async void Head_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {

            var head = Head.Value;

            var left = LeftArm.Value;

            var right = RightArm.Value;

            var bottom = Bottom.Value;

            await PlayElectronBotAsync((float)head, (float)left, (float)right, (float)bottom);


            //var list = electron.GetJointAngles();
        }

        private Task PlayElectronBotAsync(float head, float left, float right, float bottom)
        {

            electron.SetJointAngles((float)head, 0, (float)left, 0, (float)right, (float)bottom, true);

            string assetsPath = Package.Current.InstalledLocation.Path + @"\Assets\yay.jpg";

            var mat = new OpenCvSharp.Mat(assetsPath, OpenCvSharp.ImreadModes.Color);

            var mat1 = mat.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Area);

            var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

            var dataMeta = mat2.Data;

            var data = new byte[240 * 240 * 3];

            Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

            electron.SetImageSrc(data);

            //Task.Delay(500);

            electron.Sync();

            return Task.CompletedTask;
        }

        private void PlayVideoButton_Click(object sender, RoutedEventArgs e)
        {
            if (electron.Connect())
            {
                var nameList = new List<string>()
            {
                "惊恐_1进入姿势.mp4",
                "惊恐_2可循环动作.mp4",
                "惊恐_3回正.mp4"
            };

                foreach (var fileName in nameList)
                {
                    var capture = new OpenCvSharp.VideoCapture(
                        Package.Current.InstalledLocation.Path + $"\\Assets\\{fileName}");

                    while (true)
                    {
                        OpenCvSharp.Mat image = new();

                        capture.Read(image);

                        capture.Set(OpenCvSharp.VideoCaptureProperties.PosFrames,
                            capture.Get(OpenCvSharp.VideoCaptureProperties.PosFrames) + 2);

                        if (image.Empty())
                        {
                            break;
                        }
                        else
                        {
                            var mat1 = image.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Lanczos4);

                            var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                            var dataMeta = mat2.Data;

                            var data = new byte[240 * 240 * 3];

                            Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

                            electron.SetImageSrc(data);

                            electron.SetJointAngles(0, 0, 0, 0, 0, 0, true);

                            electron.Sync();
                        }
                    }
                }
            }
        }

        private void ReleaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (electron.Connect())
            {
                electron.Disconnect();
            }
        }
    }
}
