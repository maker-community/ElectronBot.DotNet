using Verdure.Braincase.Core.Models;

namespace Verdure.Braincase.Core.Contracts.Services;
public interface IEmoticonActionFrameService
{
    bool IsConnected
    {
        get;
    }
    Task<bool> SendToUsbDeviceAsync(EmoticonActionFrame data, CancellationToken cancellationToken = default);
    void ClearQueue();
}
