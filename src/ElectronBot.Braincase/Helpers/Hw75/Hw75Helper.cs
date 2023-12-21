using HelloWordKeyboard.DotNet;

namespace ElectronBot.Braincase.Helpers;

public class Hw75Helper
{
    public IHw75DynamicDevice? Hw75DynamicDevice
    {
        get; set;
    }

    private static Hw75Helper? _instance;
    public static Hw75Helper Instance => _instance ??= new Hw75Helper();

    private readonly SynchronizationContext? _context = SynchronizationContext.Current;

    public Hw75Helper()
    {
        try
        {
            Hw75DynamicDevice = new Hw75DynamicDevice();
            //Hw75DynamicDevice.Open();
            HwConnected = true;
        }
        catch
        {

        }

    }
    public bool HwConnected
    {
        get; set;
    }
}
