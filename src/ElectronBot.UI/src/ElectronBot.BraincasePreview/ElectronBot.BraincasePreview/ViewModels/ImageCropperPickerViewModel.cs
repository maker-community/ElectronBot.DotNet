using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ElectronBot.BraincasePreview.Contracts.ViewModels;
using ElectronBot.BraincasePreview.Models;
using ElectronBot.BraincasePreview.Picker;
using Microsoft.UI.Xaml.Media.Imaging;

namespace ElectronBot.BraincasePreview.ViewModels;
public class ImageCropperPickerViewModel : ObservableRecipient, INavigationAware, IObjectPicker<WriteableBitmap>
{
    private WriteableBitmap _sourceImage;
    public WriteableBitmap SourceImage
    {
        get => _sourceImage;
        set => SetProperty(ref _sourceImage, value);
    }
    private double _aspectRatio;
    public double AspectRatio
    {
        get => _aspectRatio;
        set => SetProperty(ref _aspectRatio, value);
    }
    private bool _circularCrop;

    public event EventHandler<ObjectPickedEventArgs<WriteableBitmap>> ObjectPicked;

    public event EventHandler Canceled;

    public void SetResult(WriteableBitmap result)
    {
        ObjectPicked?.Invoke(this, new ObjectPickedEventArgs<WriteableBitmap>(result));
    }
    public void Exit()
    {
        Canceled?.Invoke(this, EventArgs.Empty);
    }

    public bool CircularCrop
    {
        get => _circularCrop;
        set => SetProperty(ref _circularCrop, value);
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is ImageCropperConfig config)
        {
            var writeableBitmap = new WriteableBitmap(1, 1);
            using (var stream = await config.ImageFile.OpenReadAsync())
            {
                await writeableBitmap.SetSourceAsync(stream);
            }

            SourceImage = writeableBitmap;
            AspectRatio = config.AspectRatio;
            CircularCrop = config.CircularCrop;
        }
    }
    public void OnNavigatedFrom()
    {
    }
}
