using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using WellMind.Services;

namespace WellMind.ViewModels;

public sealed class HistoryReminderModalViewModel : BaseViewModel
{
    private readonly HistoryReminderService _historyReminderService;
    private bool _isInitialized;
    private string _eventText = "On a day like today, someone chose to slow down and notice what was already working.";
    private string _reflectionText = "Gentle progress counts. A small, steady step is still a step.";

    public HistoryReminderModalViewModel(HistoryReminderService historyReminderService)
    {
        _historyReminderService = historyReminderService;
        CloseCommand = new Command(async () => await CloseAsync());
        ShareCommand = new Command(async () => await ShareAsync());
    }

    public string Title => "A reminder from history";

    public string EventText
    {
        get => _eventText;
        private set => SetProperty(ref _eventText, value);
    }

    public string ReflectionText
    {
        get => _reflectionText;
        private set => SetProperty(ref _reflectionText, value);
    }

    public ICommand CloseCommand { get; }
    public ICommand ShareCommand { get; }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        try
        {
            var reminder = await _historyReminderService.GetForTodayAsync();
            EventText = $"On a day like today, {reminder.Event}";
            ReflectionText = reminder.Reflection;
        }
        catch
        {
            EventText = "On a day like today, someone chose to slow down and notice what was already working.";
            ReflectionText = "Gentle progress counts. A small, steady step is still a step.";
        }
    }

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

        var fileName = $"history-reminder-{DateTime.Now:yyyyMMdd-HHmmss}.png";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        await using (var imageStream = await screenshot.OpenReadAsync())
        await using (var fileStream = File.OpenWrite(filePath))
        {
            await imageStream.CopyToAsync(fileStream);
        }

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share reminder",
            File = new ShareFile(filePath)
        });
    }
}
