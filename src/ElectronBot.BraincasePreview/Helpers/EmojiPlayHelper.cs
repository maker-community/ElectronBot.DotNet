using System;
using System.Collections.Generic;
using System.Threading;
using Verdure.ElectronBot.Core.Models;

namespace ElectronBot.BraincasePreview.Helpers;
public class EmojiPlayHelper
{
    private static EmojiPlayHelper _current;
    public static EmojiPlayHelper Current => _current ??= new EmojiPlayHelper();

    private readonly Queue<EmoticonActionFrame> _actonFrame = new();

    private readonly object _actonFrameLock = new();

    public int Interval
    {
        get; set;
    }

    public void Start()
    {
        var thread = new Thread(RunPlayAsync);

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
                    if (_actonFrame.Count > 0)
                    {
                        var frame = _actonFrame.Dequeue();



                        if (ElectronBotHelper.Instance.EbConnected)
                        {
                            try
                            {
                                if (ElectronBotHelper.Instance.EbConnected)
                                {

                                    ElectronBotHelper.Instance.PlayEmoticonActionFrame(frame);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        Thread.Sleep(Interval);
                    }
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
