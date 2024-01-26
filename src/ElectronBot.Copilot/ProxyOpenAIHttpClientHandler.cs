using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronBot.Copilot;

public class ProxyOpenAIHttpClientHandler : HttpClientHandler
{
    private readonly string _proxyUrl;

    public ProxyOpenAIHttpClientHandler(string proxyUrl)
        => _proxyUrl = proxyUrl.TrimEnd('/');

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri != null && request.RequestUri.Host.Equals("api.openai.com", StringComparison.OrdinalIgnoreCase))
        {
            var path = request.RequestUri.PathAndQuery.TrimStart('/');
            request.RequestUri = new Uri($"{_proxyUrl}/{path}");
        }

        return base.SendAsync(request, cancellationToken);
    }
}
