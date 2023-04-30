using System.Collections.Concurrent;
using ElectronBot.Braincase.Helpers;
using Verdure.ElectronBot.Core.Models;

namespace Services;
public class EmoticonActionFrameService
{
    private readonly ConcurrentQueue<(EmoticonActionFrame Data, TaskCompletionSource<bool> Tcs, CancellationToken Ct)> _queue = new();

    private int _isSending;

    public async Task<bool> SendToUsbDeviceAsync(EmoticonActionFrame data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tcs = new TaskCompletionSource<bool>();

        _queue.Enqueue((data, tcs, cancellationToken));

        if (Interlocked.CompareExchange(ref _isSending, 1, 0) == 0)
        {
            _ = Task.Run(SendDataAsync, CancellationToken.None);
        }

        return await tcs.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    public void ClearQueue()
    {
        Interlocked.Exchange(ref _isSending, 0);
        _queue.Clear();
    }

    private Task SendDataAsync()
    {
        while (_queue.TryDequeue(out var item))
        {
            var (data, tcs, cancellationToken) = item;

            if (cancellationToken.IsCancellationRequested)
            {
                tcs.TrySetCanceled(cancellationToken);
                continue;
            }

            try
            {
                if (data != null)
                {
                    ElectronBotHelper.Instance.PlayEmoticonActionFrame(data);
                    tcs.TrySetResult(true);
                }
                //todo:
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }
        }

        Interlocked.Exchange(ref _isSending, 0);

        return Task.CompletedTask;
    }
}