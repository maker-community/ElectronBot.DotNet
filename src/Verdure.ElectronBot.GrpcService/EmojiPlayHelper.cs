using System.Diagnostics;
using ElectronBot.DotNet;
using Verdure.ElectronBot.Core.Models;

namespace Verdure.ElectronBot.GrpcService;
public class EmojiPlayHelper
{
    private static EmojiPlayHelper _current;
    public static EmojiPlayHelper Current => _current ??= new EmojiPlayHelper();

    private readonly Queue<EmoticonActionFrame> _actonFrame = new();

    private readonly object _actonFrameLock = new();

    public IElectronLowLevel ElectronLowLevel
    {
        get; set;
    }

    public int Interval
    {
        get; set;
    }

    public bool CanPlay
    {
        get; set;
    }

    public void Start()
    {
        var thread = new Thread(RunPlayAsync);

        ElectronLowLevel.Connect();
        //thread.IsBackground = true;
        thread.Start();
    }

    public void Clear()
    {
        _actonFrame.Clear();
    }

    private void RunPlayAsync()
    {
        while (true)
        {
            try
            {
                lock (_actonFrameLock)
                {
                    if (_actonFrame.Count > 10)
                    {
                        CanPlay = true;
                    }

                    if (CanPlay)
                    {
                        var stopwatch = Stopwatch.StartNew();

                        stopwatch.Start();

                        var frame = _actonFrame.Dequeue();


                        if (ElectronLowLevel.IsConnected)
                        {
                            ElectronLowLevel.SetImageSrc(frame.FrameBuffer);
                            ElectronLowLevel.SetJointAngles(frame.J1, frame.J2, frame.J3, frame.J4, frame.J5, frame.J6, frame.Enable);
                            ElectronLowLevel.Sync();
                        }
                        stopwatch.Stop();

                        Console.WriteLine($"time- send time{stopwatch.ElapsedMilliseconds}");
                    }

                    if (_actonFrame.Count == 0)
                    {
                        CanPlay = false;
                    }
                    Thread.Sleep(Interval);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }

    public void Enqueue(EmoticonActionFrame frame)
    {
        _actonFrame.Enqueue(frame);
    }
}
