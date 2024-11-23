using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BotSharp.Abstraction.Agents.Enums;
using BotSharp.Abstraction.Conversations.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenAI.Assistants;
using Verdure.Braincase.Core.Models.Lingxi;

namespace Verdure.Braincase.Copilot.ViewModels;
public partial class LingxiSpaceItemViewModel: ObservableRecipient
{
    private readonly Func<LingxiSpace, Task> _editFunc;
    private readonly Func<LingxiSpace, Task> _deleteFunc;

    private readonly LingxiSpace Data;

    public LingxiSpaceItemViewModel(LingxiSpace space,
        Func<LingxiSpace, Task> editFunc,
        Func<LingxiSpace, Task> deleteFunc)
    {
        Data = space;
        Id = space.Id;
        Name = space.Name;
        Desc = space.Desc;
        CreatedTime = space.CreatedTime;
        ConversationId = space.ConversationId;
        _editFunc = editFunc;
        _deleteFunc = deleteFunc;
    }
}
