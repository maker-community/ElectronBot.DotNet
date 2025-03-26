// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using BotSharp.Abstraction.Agents.Models;

namespace Verdure.Braincase.Copilot.Controls.Agents;
public sealed partial class AgentEditControl : UserControl
{
    public Agent Agent
    {
        get;
    }

    public AgentEditControl(Agent agent)
    {
        Agent = agent;
        this.InitializeComponent();
    }
}
