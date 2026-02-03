using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;

namespace WellMind.ViewModels;

public sealed class DailyAffirmationsModalViewModel : BaseViewModel
{
    public DailyAffirmationsModalViewModel()
    {
        CloseCommand = new Command(async () => await CloseAsync());
        ShareCommand = new Command(async () => await ShareAsync());
    }

    public ICommand CloseCommand { get; }
    public ICommand ShareCommand { get; }

    private async Task CloseAsync()
    {
        await Shell.Current.Navigation.PopModalAsync();
    }

    private async Task ShareAsync()
    {
        var screenshot = await Screenshot.Default.CaptureAsync();
        if (screenshot is null)
        {
            return;
        }

        var fileName = $"daily-affirmations-{DateTime.Now:yyyyMMdd-HHmmss}.png";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        await using (var imageStream = await screenshot.OpenReadAsync())
        await using (var fileStream = File.OpenWrite(filePath))
        {
            await imageStream.CopyToAsync(fileStream);
        }

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share affirmations",
            File = new ShareFile(filePath)
        });
    }
}
