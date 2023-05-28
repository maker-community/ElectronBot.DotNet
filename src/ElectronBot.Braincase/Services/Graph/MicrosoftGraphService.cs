using System.Net.Http.Headers;
using Verdure.ElectronBot.Core.Contracts.Services;
using Verdure.ElectronBot.Core.Helpers;
using Microsoft.Graph;

namespace ElectronBot.Braincase.Services;

public class MicrosoftGraphService : IMicrosoftGraphService
{
    //// For more information about Get-User Service, refer to the following documentation
    //// https://docs.microsoft.com/graph/api/user-get?view=graph-rest-1.0
    //// You can test calls to the Microsoft Graph with the Microsoft Graph Explorer
    //// https://developer.microsoft.com/graph/graph-explorer

    private const string _graphAPIEndpoint = "https://graph.microsoft.com/v1.0/";

    private readonly IdentityService _identityService;

    private readonly GraphServiceClient _graphServiceClient;

    public MicrosoftGraphService(IdentityService identityService)
    {
        _identityService = identityService;
        _graphServiceClient = new GraphServiceClient(_graphAPIEndpoint,
            new DelegateAuthenticationProvider(async (requestMessage) =>
            {
                var accessToken = await _identityService.GetAccessTokenForGraphAsync();

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            }));
    }

    public Task PrepareGraphAsync()
    {
      

        return Task.CompletedTask;
    }

    public async Task<User> GetUserInfoAsync()
    {
        var graphUser = await _graphServiceClient.Me.Request().GetAsync();

        return graphUser;
    }

    public async Task<string> GetUserPhotoAsync()
    {
        var stream = await _graphServiceClient.Me.Photo.Content
            .Request()
            .GetAsync();
        return stream.ToBase64String();
    }

    public async Task<IList<TodoTaskList>> GetTodoTaskListAsync()
    {
        return await _graphServiceClient.Me.Todo.Lists.Request().GetAsync();
    }
    public async Task<IList<TodoTask>> GetTodoTaskListByTaskIdAsync(string id)
    {
        return await _graphServiceClient.Me.Todo.Lists[id].Tasks.Request().GetAsync();
    }
}
