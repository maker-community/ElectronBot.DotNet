using Verdure.Braincase.Core.Models;

namespace Verdure.Braincase.Core.Contracts.Services;
public interface IEmoticonActionFrameService
{
    Task<bool> SendToUsbDeviceAsync(EmoticonActionFrame data, CancellationToken cancellationToken = default);
    void ClearQueue();
}
