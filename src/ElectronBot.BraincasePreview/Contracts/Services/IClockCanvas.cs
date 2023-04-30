using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.UI.Xaml.Media.Imaging;

namespace ElectronBot.Braincase.Contracts.Services;
public interface IClockCanvas
{
    Task<BitmapImage> CreateCanvasImageAsync(CanvasDevice device, CancellationToken cancellationToken = default);
   void CreateCanvasImage(CanvasRenderTarget renderTarget, CancellationToken cancellationToken = default);
}
