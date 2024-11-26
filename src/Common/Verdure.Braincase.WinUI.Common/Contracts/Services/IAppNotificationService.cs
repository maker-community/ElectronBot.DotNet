using System.Collections.Specialized;

namespace Verdure.Braincase.WinUI.Common.Contracts.Services;

public interface IAppNotificationService
{
    void Initialize();

    bool Show(string payload);

    NameValueCollection ParseArguments(string arguments);

    void Unregister();
}
