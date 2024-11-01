using Verdure.Braincase.Core.Contracts.Services;

namespace Verdure.Braincase.Core.Contracts.Services;
public interface IBotIotService
{
    Task PostServiceAync(IotRequest request);
}
