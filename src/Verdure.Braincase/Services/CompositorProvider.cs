using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Controls;
using Verdure.Braincase;

namespace Services;
internal class CompositorProvider : ICompositorProvider
{
    public Compositor GetCompositor()
    {
        return App.MainWindow.Compositor;
    }

    public Frame GetRootFrame() => App.RootFrame;

    public WindowEx GetWindow()
    {
        return App.MainWindow;
    }
}
