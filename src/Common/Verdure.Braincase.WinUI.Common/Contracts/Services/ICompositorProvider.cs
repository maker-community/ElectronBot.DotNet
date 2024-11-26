using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;

namespace Verdure.Braincase.WinUI.Common.Contracts.Services;
public interface ICompositorProvider
{
    Compositor GetCompositor();
    WindowEx GetWindow();

    Frame GetRootFrame();
}
