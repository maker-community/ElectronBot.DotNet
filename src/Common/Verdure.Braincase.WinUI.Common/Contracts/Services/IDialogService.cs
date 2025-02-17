using Microsoft.UI.Xaml.Controls;

namespace Verdure.Braincase.WinUI.Common.Contracts.Services;
public interface IDialogService
{
    Task<ContentDialogResult> ShowDialogAsync(string title, string primaryButtonText, string closeButtonText, object content);
}
