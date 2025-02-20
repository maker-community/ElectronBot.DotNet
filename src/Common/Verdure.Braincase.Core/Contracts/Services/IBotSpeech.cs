namespace Verdure.Braincase.Core.Contracts.Services;
public interface IBotSpeech : IDisposable
{
    Task<string> ListenAsync(CancellationToken cancellationToken = default);

    Task SpeakAsync(string text, CancellationToken cancellationToken = default);
}
