using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Verdure.Braincase.Copilot.ViewModels;
public partial class LingxiSpaceItemViewModel: ObservableRecipient
{
    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _desc;

    [ObservableProperty]
    private string _type;

    [ObservableProperty]
    private string? _imageData;

    [ObservableProperty]

    private DateTime _createdTime;

    [ObservableProperty]
    private string _conversationId;
}
