namespace Verdure.Braincase.Core.Contracts.Services;
public interface IWakeWordListener : IDisposable
{
    Task<bool> WaitForWakeWordAsync(CancellationToken cancellationToken);
}
