using CommunityToolkit.Mvvm.ComponentModel;
using Google.Protobuf.WellKnownTypes;
using Verdure.Braincase.WinUI.Common.Contracts.Services;

namespace Verdure.Braincase.EBConfiguration.ViewModels;

public partial class EBDebugViewModel : ObservableRecipient
{
    private readonly IEmoticonActionFrameService _actionFrameService;

    private readonly IntPtr _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow());

    private int modeNo = 0;

    private int count = 0;

    private int actionCount = 0;
    public EBDebugViewModel(IEmoticonActionFrameService actionFrameService)
    {
        _actionFrameService = actionFrameService;
    }
}
