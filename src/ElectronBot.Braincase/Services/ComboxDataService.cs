using System.Collections.ObjectModel;
using Verdure.ElectronBot.Core.Models;
using ElectronBot.Braincase.Helpers;

namespace ElectronBot.Braincase.Services;
public class ComboxDataService
{
    public ObservableCollection<ComboxItemModel> GetClockViewComboxList()
    {
        return new ObservableCollection<ComboxItemModel>
            {

                new ComboxItemModel { DataKey = "DefautView", DataValue = "DefautView".GetLocalized() },
                new ComboxItemModel { DataKey = "LongShadowView", DataValue ="LongShadowView".GetLocalized() },
                new ComboxItemModel { DataKey = "GooeyFooter", DataValue ="GooeyFooter".GetLocalized() },
                new ComboxItemModel { DataKey = "GradientsWithBlend", DataValue ="GradientsWithBlend".GetLocalized() },
                new ComboxItemModel { DataKey = "CustomView", DataValue = "CustomView".GetLocalized() }
            };
    }

    public ObservableCollection<ComboxItemModel> GetHw75ViewComboxList()
    {
        return new ObservableCollection<ComboxItemModel>
            {

                new ComboxItemModel { DataKey = "Hw75CustomView", DataValue = "Hw75CustomView".GetLocalized() },
                new ComboxItemModel { DataKey = "Hw75WeatherView", DataValue ="Hw75WeatherView".GetLocalized() },
                new ComboxItemModel { DataKey = "Hw75YellowCalendarView", DataValue ="Hw75YellowCalendarView".GetLocalized() },
                //new ComboxItemModel { DataKey = "TodoView", DataValue ="TodoView".GetLocalized() },
            };
    }

    public ObservableCollection<ComboxItemModel> GetChatBotClientComboxList()
    {
        return new ObservableCollection<ComboxItemModel>
            {

                new ComboxItemModel { DataKey = "Turing", DataValue = "TuringTitle".GetLocalized() },
                new ComboxItemModel { DataKey = "ChatGPT", DataValue ="ChatGPTTitle".GetLocalized() }
            };
    }
}
