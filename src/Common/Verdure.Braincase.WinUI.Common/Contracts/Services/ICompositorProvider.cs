using Microsoft.UI.Composition;
using WinUIEx;

namespace Verdure.Braincase.WinUI.Common.Contracts.Services;
public interface ICompositorProvider
{
    Compositor GetCompositor();
    WindowEx GetWindow();
}
