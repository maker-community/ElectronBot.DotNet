using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace ElectronBot.Braincase.Models;

public class ElectronBotAction : ObservableRecipient
{
    private float j1 = 0;
    private float j2 = 0;
    private float j3 = 0;
    private float j4 = 0;
    private float j5 = 0;
    private float j6 = 0;

    private WriteableBitmap _bitmapImageData;
    public float J1
    {
        get => j1;
        set => SetProperty(ref j1, value);
    }
    public float J2
    {
        get => j2;
        set => SetProperty(ref j2, value);
    }
    public float J3
    {
        get => j3;
        set => SetProperty(ref j3, value);
    }
    public float J4
    {
        get => j4;
        set => SetProperty(ref j4, value);
    }
    public float J5
    {
        get => j5;
        set => SetProperty(ref j5, value);
    }
    public float J6
    {
        get => j6;
        set => SetProperty(ref j6, value);
    }
    public bool Enable
    {
        get; set;
    }
    [JsonIgnore]
    public string Id
    {
        get; set;
    }
    public string ImageData
    {
        get; set;
    }
    [JsonIgnore]
    public WriteableBitmap BitmapImageData
    {
        get => _bitmapImageData; set => SetProperty(ref _bitmapImageData, value);
    }
}
