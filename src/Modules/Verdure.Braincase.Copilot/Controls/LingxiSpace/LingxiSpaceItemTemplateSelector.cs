using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verdure.Braincase.Copilot.ViewModels;
using Verdure.Braincase.Core.Models.Lingxi;

namespace Verdure.Braincase.Copilot.Controls.LingxiSpace;
public class LingxiSpaceItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate TextTemplate
    {
        get; set;
    }
    public DataTemplate ImageTemplate
    {
        get; set;
    }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is LingxiSpaceItemViewModel viewModel)
        {
            return viewModel.Type != LingxiSpaceType.Word ? ImageTemplate : TextTemplate;
        }
        return base.SelectTemplateCore(item, container);
    }
}