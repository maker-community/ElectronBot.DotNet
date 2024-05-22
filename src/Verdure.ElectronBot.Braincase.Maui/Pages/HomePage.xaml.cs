using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verdure.ElectronBot.Braincase.Maui.ViewModels;

namespace Verdure.ElectronBot.Braincase.Maui.Pages;

public partial class HomePage : ContentPage
{
    public HomeViewModel ViewModel { get; set; }
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
    }
}