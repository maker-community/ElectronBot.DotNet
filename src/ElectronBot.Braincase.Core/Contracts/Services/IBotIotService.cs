using Verdure.ElectronBot.Core.Models.Iot;

namespace Verdure.ElectronBot.Core.Contracts.Services;
public interface IBotIotService
{
    Task PostServiceAync(IotRequest request);
}
