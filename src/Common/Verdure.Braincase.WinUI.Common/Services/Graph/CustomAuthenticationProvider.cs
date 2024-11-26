using Verdure.Braincase.Services;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Threading;

namespace Verdure.Braincase.Core.Services.Graph;
public class CustomAuthenticationProvider : IAuthenticationProvider
{
    private readonly IdentityService _identityService;
    public CustomAuthenticationProvider(IdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await _identityService.GetAccessTokenForGraphAsync();

        request.Headers.Add("Authorization", $"bearer {accessToken}");
    }
}
