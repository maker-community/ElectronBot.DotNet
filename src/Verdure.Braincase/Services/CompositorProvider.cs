using Microsoft.UI.Composition;
using Verdure.Braincase;

namespace Services;
internal class CompositorProvider : ICompositorProvider
{
    public Compositor GetCompositor()
    {
        return App.MainWindow.Compositor;
    }
}
