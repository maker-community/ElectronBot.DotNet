using System;
using System.Threading;
using System.Threading.Tasks;
using ElectronBot.Braincase.Contracts.Services;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation;

namespace ElectronBot.Braincase.Services;
public class DefaultClockCanvas : IClockCanvas
{
    private readonly string _name;

    public DefaultClockCanvas(string name)
    {
        _name = name;
    }

    public Task<BitmapImage> CreateCanvasImageAsync(CanvasDevice device, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void CreateCanvasImage(
        CanvasRenderTarget renderTarget, CancellationToken cancellationToken = default)
    {

        using var ds = renderTarget.CreateDrawingSession();
        DrawTile(ds, 240, 240);
    }


    private static void DrawTile(CanvasDrawingSession ds, float width, float height)
    {
        using var cl = new CanvasCommandList(ds);

        using (var clds = cl.CreateDrawingSession())
        {
            var text = string.Format("{0}\n{1}", DateTime.Now.ToString("ddd"), DateTime.Now.ToString("HH:mm:ss"));

            var textFormat = new CanvasTextFormat()
            {
                FontFamily = "Segoe UI Black",
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center,
                FontSize = 24,
                LineSpacing = 24
            };

            clds.DrawText(text, 0, 0, Colors.White, textFormat);
        }

        var effect = new GaussianBlurEffect()
        {
            Source = cl,
            BlurAmount = 1,
        };

        ds.Clear(Colors.Orange);

        var bounds = effect.GetBounds(ds);

        var ratio = bounds.Height / bounds.Width;

        var destHeight = height * ratio;

        ds.DrawImage(effect, new Rect(0, height / 2 - destHeight / 2, width, destHeight), bounds);

        ds.DrawText(string.Format("电子脑壳\n{0}\n{1}", DateTime.Now.ToString("d"), DateTime.Now.ToString("HH:mm:ss")),
            12, 12, Colors.Purple,
            new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                VerticalAlignment = CanvasVerticalAlignment.Top,
                FontSize = 16
            });
    }
}
