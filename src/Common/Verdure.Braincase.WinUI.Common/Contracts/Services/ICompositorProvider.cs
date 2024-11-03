using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Composition;

namespace Verdure.Braincase.WinUI.Common.Contracts.Services;
public interface ICompositorProvider
{
    Compositor GetCompositor();
}
