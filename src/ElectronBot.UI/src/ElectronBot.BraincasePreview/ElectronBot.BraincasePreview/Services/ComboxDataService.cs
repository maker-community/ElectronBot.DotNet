using System.Collections.ObjectModel;
using ElectronBot.BraincasePreview.Core.Models;
using ElectronBot.BraincasePreview.Helpers;

namespace ElectronBot.BraincasePreview.Services;
public class ComboxDataService
{
    public ObservableCollection<ComboxItemModel> GetClockViewComboxList()
    {
        return new ObservableCollection<ComboxItemModel>
            {

                new ComboxItemModel { DataKey = "DefautView", DataValue = "DefautView".GetLocalized() },
                new ComboxItemModel { DataKey = "LongShadowView", DataValue ="LongShadowView".GetLocalized() },
                new ComboxItemModel { DataKey = "GooeyFooter", DataValue ="GooeyFooter".GetLocalized() }
            };
    }

}
