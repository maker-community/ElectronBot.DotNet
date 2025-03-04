namespace Verdure.Braincase.Core.Contracts.Services;
public interface IBotSpeech : IDisposable
{
    string Provider
    {
        get;
    }

    Task InitAsync(CancellationToken cancellationToken = default);
    Task<string> ListenAsync(CancellationToken cancellationToken = default);

    Task SpeakAsync(string text, CancellationToken cancellationToken = default);
}
