using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Verdure.Braincase.Copilot.ViewModels;
public partial class LingxiSpaceViewModel : ObservableRecipient
{
    public LingxiSpaceViewModel()
    {

    }

    [ObservableProperty]
    private bool _isLingxiEmpty;

    [ObservableProperty]
    ObservableCollection<LingxiSpaceItemViewModel> _lingxiSpaceList = new();
}
