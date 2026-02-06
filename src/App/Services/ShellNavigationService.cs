using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using WellMind.Views;

namespace WellMind.Services;

public sealed class ShellNavigationService : INavigationService
{
    private readonly IServiceProvider _services;

    public ShellNavigationService(IServiceProvider services)
    {
        _services = services;
    }

    public Task GoToCheckInAsync()
    {
        return Shell.Current.GoToAsync(nameof(CheckInPage));
    }

    public Task GoBackAsync()
    {
        return Shell.Current.GoToAsync("..");
    }

    public Task GoToResourceAsync(string title, string url)
    {
        var encodedTitle = Uri.EscapeDataString(title ?? string.Empty);
        var encodedUrl = Uri.EscapeDataString(url ?? string.Empty);
        return Shell.Current.GoToAsync($"{nameof(InAppBrowserPage)}?title={encodedTitle}&url={encodedUrl}");
    }

    public async Task OpenGentleReminderAsync()
    {
        var page = _services.GetRequiredService<GentleReminderPage>();
        await Shell.Current.Navigation.PushModalAsync(page);
    }

    public async Task OpenHistoryReminderAsync()
    {
        var page = _services.GetRequiredService<HistoryReminderModalPage>();
        await Shell.Current.Navigation.PushModalAsync(page);
    }

    public Task CloseModalAsync()
    {
        var shell = Shell.Current;
        if (shell?.Navigation is null)
        {
            return Task.CompletedTask;
        }

        if (shell.Navigation.ModalStack.Count == 0)
        {
            return Task.CompletedTask;
        }

        return MainThread.InvokeOnMainThreadAsync(() => shell.Navigation.PopModalAsync());
    }
}
