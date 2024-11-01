using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Verdure.Braincase.Core.Contracts.Services;

namespace Verdure.Braincase.Services;

public class MicrosoftGraphService : IMicrosoftGraphService
{
    //// For more information about Get-User Service, refer to the following documentation
    //// https://docs.microsoft.com/graph/api/user-get?view=graph-rest-1.0
    //// You can test calls to the Microsoft Graph with the Microsoft Graph Explorer
    //// https://developer.microsoft.com/graph/graph-explorer

    private const string _graphAPIEndpoint = "https://graph.microsoft.com/v1.0/";

    private readonly GraphServiceClient _graphServiceClient;

    public MicrosoftGraphService(IAuthenticationProvider authenticationProvider)
    {
        _graphServiceClient = new GraphServiceClient(authenticationProvider, _graphAPIEndpoint);
    }

    public Task PrepareGraphAsync()
    {
        return Task.CompletedTask;
    }

    public async Task<User> GetUserInfoAsync()
    {
        var graphUser = await _graphServiceClient.Me.GetAsync();

        return graphUser;
    }

    public async Task<string> GetUserPhotoAsync()
    {
        var stream = await _graphServiceClient.Me.Photo.Content
            .GetAsync();
        return stream.ToBase64String();
    }

    public async Task<IList<TodoTaskList>> GetTodoTaskListAsync()
    {
        var todoTaskLists = await _graphServiceClient.Me.Todo.Lists.GetAsync();
        return todoTaskLists?.Value ?? new List<TodoTaskList>();
    }
    public async Task<IList<TodoTask>> GetTodoTaskListByTaskIdAsync(string id)
    {
        var tasksResponse = await _graphServiceClient.Me.Todo.Lists[id].Tasks.GetAsync();
        return tasksResponse?.Value ?? new List<TodoTask>();
    }
}
