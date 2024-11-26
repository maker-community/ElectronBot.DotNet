using BotSharp.Abstraction.Agents;
using Verdure.Braincase.Copilot.Plugin.Enums;
using Verdure.Braincase.Copilot.Plugin.Hooks;

namespace Verdure.Braincase.Copilot.Plugin;

public class VerdureCopilotPlugin : IBotSharpPlugin
{
    public string Id => "dfc60ce4-fce4-40a7-b0e2-f945c95e19dc";
    public string Name => "Verdure Copilot";
    public string Description => "help user";
    public string IconUrl => "https://cdn-icons-png.flaticon.com/512/6978/6978255.png";
    public string[] AgentIds => new[]
    {
        VerdureAgentId.VerdureId,
        VerdureAgentId.VerdureLearnlId,
        VerdureAgentId.VerdureToolId,
        VerdureAgentId.VerdureChatId
    };

    public void RegisterDI(IServiceCollection services, IConfiguration config)
    {
        // Register hooks
        services.AddScoped<IAgentHook, VerdureCopilotAgentHook>();
    }
}
