using BotSharp.Abstraction.Agents;
using BotSharp.Abstraction.Agents.Enums;

namespace Verdure.Braincase.Copilot.Plugin.Hooks;

public class VerdureCopilotAgentHook : AgentHookBase
{
    public override string SelfId => BuiltInAgentId.AIAssistant;

    public VerdureCopilotAgentHook(IServiceProvider services, AgentSettings settings)
        : base(services, settings)
    {
    }

    public override bool OnInstructionLoaded(string template, Dictionary<string, object> dict)
    {
        return base.OnInstructionLoaded(template, dict);
    }
}
